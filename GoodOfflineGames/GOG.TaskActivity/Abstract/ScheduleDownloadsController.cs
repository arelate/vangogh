using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.Destination;
using Interfaces.DownloadSources;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ScheduleDownloadsController : TaskActivityController
    {
        internal string destination;
        internal ScheduledDownloadTypes downloadType;

        private IDownloadSourcesController downloadSourcesController;
        private IProductTypeStorageController productTypeStorageController;
        private ICollectionController collectionController;
        private IFileController fileController;
        private IDestinationController destinationController;

        public ScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IDestinationController destinationController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.downloadSourcesController = downloadSourcesController;
            this.productTypeStorageController = productTypeStorageController;
            this.collectionController = collectionController;
            this.destinationController = destinationController;
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
                foreach (var source in downloadSource.Value)
                {
                    var existingProductDownload = collectionController.Find(scheduledDownloads, sd =>
                        sd != null &&
                        sd.Source == source);

                    if (existingProductDownload != null) continue;

                    if (destinationController != null &&
                        fileController != null)
                    {
                        var localFile = destinationController.GetDestination(source, destination);
                        if (fileController.Exists(localFile)) continue;
                    }

                    var newScheduledDownload = new ScheduledDownload();
                    newScheduledDownload.Id = downloadSource.Key;
                    newScheduledDownload.Type = downloadType;
                    newScheduledDownload.Source = source;
                    newScheduledDownload.Destination = destination;

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
