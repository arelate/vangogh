using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.SourceDestination;
using Interfaces.File;
using Interfaces.Reporting;
using Interfaces.UriResolution;
using Interfaces.DestinationAdjustment;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductFiles
{
    public class ProductFilesScheduleDownloadsController: ScheduleDownloadsController
    {
        public ProductFilesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IUriResolutionController uriResolutionController,
            IDestinationAdjustmentController destinationAdjustmentController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            ISourceDestinationController sourceDestinationController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base (
                Models.Custom.ScheduledDownloadTypes.File,
                string.Empty,
                downloadSourcesController,
                uriResolutionController,
                destinationAdjustmentController,
                productTypeStorageController,
                collectionController,
                sourceDestinationController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
