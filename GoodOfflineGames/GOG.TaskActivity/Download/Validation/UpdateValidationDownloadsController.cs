using Interfaces.DownloadSources;
using Interfaces.Reporting;
using Interfaces.Destination;
using Interfaces.Data;

using GOG.Models;
using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Validation
{
    public class UpdateValidationDownloadsController: UpdateDownloadsController
    {
        public UpdateValidationDownloadsController(
            ProductDownloadTypes productDownloadType,
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskReportingController taskReportingController) :
            base(productDownloadType,
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
