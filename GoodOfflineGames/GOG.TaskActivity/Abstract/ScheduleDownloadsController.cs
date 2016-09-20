using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.DownloadSources;
using Interfaces.UriRedirect;
using Interfaces.Destination;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ScheduleDownloadsController : TaskActivityController
    {
        private ScheduledDownloadTypes downloadType;

        private IDownloadSourcesController downloadSourcesController;
        private IUriRedirectController uriRedirectController;
        private IDestinationController destinationController;
        private IProductTypeStorageController productTypeStorageController;
        private ICollectionController collectionController;
        private IFileController fileController;

        public ScheduleDownloadsController(
            ScheduledDownloadTypes downloadType,
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IDestinationController destinationController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.downloadSourcesController = downloadSourcesController;
            this.uriRedirectController = uriRedirectController;
            this.destinationController = destinationController;
            this.productTypeStorageController = productTypeStorageController;
            this.collectionController = collectionController;
            this.fileController = fileController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Load existing scheduled downloads");
            var scheduledDownloads = await productTypeStorageController.Pull<ScheduledDownload>(ProductTypes.ScheduledDownload);
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Get " + System.Enum.GetName(typeof(ScheduledDownloadTypes), downloadType) + " sources");
            var downloadSources = await downloadSourcesController.GetDownloadSources();
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Schedule downloads if not previously scheduled and no file exists");
            foreach (var downloadSource in downloadSources)
            {
                var productId = downloadSource.Key;

                foreach (var source in downloadSource.Value)
                {
                    var existingProductDownload = collectionController.Find(scheduledDownloads, sd =>
                        sd != null &&
                        sd.Source == source);

                    if (existingProductDownload != null) continue;

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

                    var newScheduledDownload = new ScheduledDownload();
                    newScheduledDownload.Id = productId;
                    newScheduledDownload.Type = downloadType;
                    newScheduledDownload.Source = actualSource;
                    newScheduledDownload.Destination = destinationDirectory;

                    scheduledDownloads.Add(newScheduledDownload);
                }
            }
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Save scheduled downloads");
            await productTypeStorageController.Push(ProductTypes.ScheduledDownload, scheduledDownloads);
            taskReportingController.CompleteTask();

        }
    }
}
