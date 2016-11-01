using Interfaces.DownloadSources;
using Interfaces.File;
using Interfaces.Reporting;
using Interfaces.UriRedirection;
using Interfaces.Destination;
using Interfaces.Data;

using GOG.Models;
using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductFiles
{
    public class ProductFilesScheduleDownloadsController: ScheduleDownloadsController
    {
        public ProductFilesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IDestinationController destinationController,
            IDataController<ScheduledDownload> scheduledDownloadsDataController,
            IDataController<Product> productsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base (
                ScheduledDownloadTypes.File,
                downloadSourcesController,
                uriRedirectController,
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
