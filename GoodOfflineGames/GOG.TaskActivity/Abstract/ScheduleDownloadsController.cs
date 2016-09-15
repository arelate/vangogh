using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.SourceDestination;
using Interfaces.DownloadSources;
using Interfaces.DestinationAdjustment;
using Interfaces.UriResolution;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ScheduleDownloadsController : TaskActivityController
    {
        private ScheduledDownloadTypes downloadType;
        private string destination;

        private IDownloadSourcesController downloadSourcesController;
        private IUriResolutionController uriResolutionController;
        private IDestinationAdjustmentController destinationAdjustmentController;
        private IProductTypeStorageController productTypeStorageController;
        private ICollectionController collectionController;
        private ISourceDestinationController sourceDestinationController;
        private IFileController fileController;

        public ScheduleDownloadsController(
            ScheduledDownloadTypes downloadType,
            string destination,
            IDownloadSourcesController downloadSourcesController,
            IUriResolutionController uriResolutionController,
            IDestinationAdjustmentController destinationAdjustmentController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            ISourceDestinationController sourceDestinationController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.downloadSourcesController = downloadSourcesController;
            this.uriResolutionController = uriResolutionController;
            this.destinationAdjustmentController = destinationAdjustmentController;
            this.productTypeStorageController = productTypeStorageController;
            this.collectionController = collectionController;
            this.sourceDestinationController = sourceDestinationController;
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

                    var resolvedSource =
                        uriResolutionController != null ?
                        await uriResolutionController.ResolveUri(source) :
                        source;

                    var adjustedDestination =
                        destinationAdjustmentController != null ?
                        destinationAdjustmentController.AdjustDestination(resolvedSource) :
                        destination;

                    if (sourceDestinationController != null &&
                        fileController != null)
                    {
                        var localFile = sourceDestinationController.GetSourceDestination(source, adjustedDestination);
                        if (fileController.Exists(localFile)) continue;
                    }

                    var newScheduledDownload = new ScheduledDownload();
                    newScheduledDownload.Id = productId;
                    newScheduledDownload.Type = downloadType;
                    newScheduledDownload.Source = resolvedSource;
                    newScheduledDownload.Destination = adjustedDestination;

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
