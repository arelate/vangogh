using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.ImageUri;
using Interfaces.DownloadSources;
using Interfaces.Destination;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductImages
{
    public class ProductImagesScheduleDownloadsController : ScheduleDownloadsController
    {
        public ProductImagesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                Models.Custom.ScheduledDownloadTypes.Image,
                downloadSourcesController, 
                null, // uriRedirectController
                destinationController, // destinationAdjustmentController
                productTypeStorageController,
                collectionController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
