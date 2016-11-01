using Interfaces.Reporting;
using Interfaces.File;
using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;

using GOG.Models;
using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductImages
{
    public class ProductImagesScheduleDownloadsController : ScheduleDownloadsController
    {
        public ProductImagesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ScheduledDownload> scheduledDownloadsDataController,
            IDataController<Product> productsDataController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                ScheduledDownloadTypes.Image,
                downloadSourcesController, 
                null, // uriRedirectController
                destinationController, // destinationAdjustmentController
                scheduledDownloadsDataController,
                productsDataController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
