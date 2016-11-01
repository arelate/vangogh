using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.DownloadSources;
using Interfaces.UriRedirection;
using Interfaces.Destination;
using Interfaces.Data;

using GOG.Models;
using GOG.Models.Custom;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ScheduleDownloadsController : TaskActivityController
    {
        private ScheduledDownloadTypes downloadType;

        private IDownloadSourcesController downloadSourcesController;
        private IUriRedirectController uriRedirectController;
        private IDestinationController destinationController;
        private IDataController<ScheduledDownload> scheduledDownloadsDataController;
        IDataController<Product> productsDataController;
        private IFileController fileController;

        private string scheduledDownloadTitle;

        public ScheduleDownloadsController(
            ScheduledDownloadTypes downloadType,
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IDestinationController destinationController,
            IDataController<ScheduledDownload> scheduledDownloadsDataController,
            IDataController<Product> productsDataController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.downloadType = downloadType;
            this.downloadSourcesController = downloadSourcesController;
            this.uriRedirectController = uriRedirectController;
            this.destinationController = destinationController;
            this.scheduledDownloadsDataController = scheduledDownloadsDataController;
            this.productsDataController = productsDataController;
            this.fileController = fileController;

            scheduledDownloadTitle = System.Enum.GetName(typeof(ScheduledDownloadTypes), downloadType);
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask(
                "Get sources for the type: {0}", 
                scheduledDownloadTitle);

            var downloadSources = await downloadSourcesController.GetDownloadSources();
            taskReportingController.CompleteTask();

            taskReportingController.StartTask(
                "Filtering previously scheduled downloads and existing files");

            foreach (var downloadSource in downloadSources)
            {
                var key = downloadSource.Key;
                var product = await productsDataController.GetById(key);
                if (product == null)
                {
                    taskReportingController.ReportWarning(
                        "Downloads are scheduled for the product that doesn't exist");
                    continue;
                }

                var scheduledDownloads = await scheduledDownloadsDataController.GetById(product.Id);
                if (scheduledDownloads == null)
                {
                    scheduledDownloads = new ScheduledDownload();
                    scheduledDownloads.Id = product.Id;
                    scheduledDownloads.Title = product.Title;
                    scheduledDownloads.Downloads = new List<ScheduledDownloadEntry>();
                }

                var productSourceAlreadyScheduled = false;

                foreach (var source in downloadSource.Value)
                {
                    // skip the source if we've already scheduled a download for same id
                    foreach (var download in scheduledDownloads.Downloads)
                        if (download.Source == source) productSourceAlreadyScheduled = true;

                    if (productSourceAlreadyScheduled) continue;

                    var actualSource =
                        uriRedirectController != null ?
                        await uriRedirectController.GetUriRedirect(source) :
                        source;

                    var destinationDirectory = destinationController?.GetDirectory(actualSource);

                    if (fileController != null)
                    {
                        var destinationFilename = destinationController?.GetFilename(actualSource);
                        var localFile = System.IO.Path.Combine(destinationDirectory, destinationFilename);
                        if (fileController.Exists(localFile)) continue;
                    }

                    taskReportingController.StartTask(
                        "Scheduling new downloads for: {0}, {1}",
                        scheduledDownloadTitle,
                        product.Title);

                    var scheduledDownloadEntry = new ScheduledDownloadEntry();
                    scheduledDownloadEntry.Type = downloadType;
                    scheduledDownloadEntry.Source = actualSource;
                    scheduledDownloadEntry.Destination = destinationDirectory;

                    scheduledDownloads.Downloads.Add(scheduledDownloadEntry);

                    await scheduledDownloadsDataController.Update(scheduledDownloads);

                    taskReportingController.CompleteTask();
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
