using Interfaces.DownloadSources;
using Interfaces.Reporting;
using Interfaces.Destination;
using Interfaces.Data;

using GOG.Models;
using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Validation
{
    public class UpdateValidationDownloadsController : UpdateDownloadsController
    {
        public UpdateValidationDownloadsController(
            IDownloadSourcesController validationSourcesController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskReportingController taskReportingController) :
            base(
                ProductDownloadTypes.Validation,
                validationSourcesController,
                destinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskReportingController)
        {
            // ...
        }
    }
}
