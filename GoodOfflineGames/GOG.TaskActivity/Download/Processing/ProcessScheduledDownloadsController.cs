using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

using Interfaces.Reporting;
using Interfaces.Download;
using Interfaces.Data;
using Interfaces.Destination;

using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Processing
{
    public class ProcessScheduledDownloadsController : TaskActivityController
    {
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<ScheduledValidation> scheduledValidationsDataController;
        private IDownloadController downloadController;
        private IDestinationController destinationController;

        public ProcessScheduledDownloadsController(
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<ScheduledValidation> scheduledValidationsDataController,
            IDownloadController downloadController,
            IDestinationController destinationController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.productDownloadsDataController = productDownloadsDataController;
            this.scheduledValidationsDataController = scheduledValidationsDataController;
            this.downloadController = downloadController;
            this.destinationController = destinationController;
        }

        public override async Task ProcessTask()
        {
            var counter = 0;
            var total = productDownloadsDataController.Count();

            var productDownloadsIds = productDownloadsDataController.EnumerateIds().ToArray();

            taskReportingController.StartTask("Process scheduled downloads");
            foreach (var id in productDownloadsIds)
            {
                var productDownloads = await productDownloadsDataController.GetById(id);

                taskReportingController.StartTask(
                        "Process downloads for product {0}/{1}: {2}",
                        ++counter,
                        total,
                        productDownloads.Title);

                var downloadEntries = productDownloads.Downloads.ToArray();

                for (var ii = 0; ii < downloadEntries.Length; ii++)
                {
                    var entry = downloadEntries[ii];

                    taskReportingController.StartTask(
                        "Download entry {0}/{1}: {2}",
                        ii + 1,
                        downloadEntries.Length,
                        System.Enum.GetName(typeof(ProductDownloadTypes), entry.Type));

                    var previousResolvedUri = entry.ResolvedUri;

                    entry.ResolvedUri = await downloadController.DownloadFile(entry.SourceUri, entry.Destination);

                    if (previousResolvedUri != entry.ResolvedUri)
                    {
                        taskReportingController.StartTask("Update resolved product Uri: {0}", productDownloads.Title);
                        await productDownloadsDataController.Update(productDownloads);
                        taskReportingController.CompleteTask();
                    }

                    taskReportingController.CompleteTask();

                    if (entry.Type == ProductDownloadTypes.ProductFile)
                    {
                        taskReportingController.StartTask("Schedule validation for downloaded file");

                        var scheduledValidation = await scheduledValidationsDataController.GetById(id);
                        if (scheduledValidation == null)
                        {
                            scheduledValidation = new ScheduledValidation();
                            scheduledValidation.Id = productDownloads.Id;
                            scheduledValidation.Title = productDownloads.Title;
                            scheduledValidation.Files = new List<string>();
                        }

                        var filePath = Path.Combine(entry.Destination,
                            destinationController.GetFilename(entry.SourceUri));

                        if (!scheduledValidation.Files.Contains(filePath))
                        {
                            scheduledValidation.Files.Add(filePath);
                            await scheduledValidationsDataController.Update(scheduledValidation);
                        }

                        taskReportingController.CompleteTask();
                    }
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
