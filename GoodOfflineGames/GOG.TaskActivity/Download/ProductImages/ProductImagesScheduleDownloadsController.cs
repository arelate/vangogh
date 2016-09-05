using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.Destination;
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
            IDestinationController destinationController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                downloadSourcesController,
                productTypeStorageController,
                collectionController,
                destinationController,
                fileController,
                taskReportingController)
        {
            downloadDescription = "product image";
            destination = "_images";
        }
    }
}
