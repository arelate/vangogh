using Interfaces.DownloadSources;
using Interfaces.Reporting;
using Interfaces.Destination;
using Interfaces.Data;

using Models.ProductDownloads;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductFiles
{
    public class UpdateFilesDownloadsController: UpdateDownloadsController
    {
        public UpdateFilesDownloadsController(
            ProductDownloadTypes productDownloadType,
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskReportingController taskReportingController) :
            base (
                productDownloadType,
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
