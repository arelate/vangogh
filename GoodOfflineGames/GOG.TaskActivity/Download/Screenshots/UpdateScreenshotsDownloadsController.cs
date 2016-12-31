using Interfaces.Reporting;
using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;

using Models.ProductDownloads;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Screenshots
{
    public class UpdateScreenshotsDownloadsController : UpdateDownloadsController
    {
        public UpdateScreenshotsDownloadsController(
            ProductDownloadTypes downloadType,
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskReportingController taskReportingController) :
            base(
                downloadType,
                downloadSourcesController,
                destinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskReportingController)
        {
            // ...
        }
    }
}
