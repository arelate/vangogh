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
    public class ImagesScheduleDownloadsController : ScheduleDownloadsController
    {
        public ImagesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ScheduledDownload> scheduledDownloadsDataController,
            IDataController<Product> productsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                ScheduledDownloadTypes.Image,
                downloadSourcesController, 
                null, // uriRedirectController
                destinationController,
                scheduledDownloadsDataController,
                productsDataController,
                accountProductsDataController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
