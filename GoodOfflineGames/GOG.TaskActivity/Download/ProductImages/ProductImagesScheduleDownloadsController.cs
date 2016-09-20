using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.ImageUri;
using Interfaces.DownloadSources;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductImages
{
    public class ProductImagesScheduleDownloadsController : ScheduleDownloadsController
    {
        public ProductImagesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IProductTypeStorageController productTypeStorageController,
            IImageUriController imageUriController,
            ICollectionController collectionController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                Models.Custom.ScheduledDownloadTypes.Image,
                "_images",
                downloadSourcesController,
                null, // uriResolutionController
                null, // destinationAdjustmentController
                productTypeStorageController,
                collectionController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
