using Interfaces.DownloadSources;
using Interfaces.File;
using Interfaces.Reporting;
using Interfaces.Destination;
using Interfaces.Data;

using GOG.Models;
using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Validation
{
    public class ValidationScheduleDownloadsController : ScheduleDownloadsController
    {
        public ValidationScheduleDownloadsController(
            IDownloadSourcesController validationSourcesController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<Product> productsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                ProductDownloadTypes.Validation,
                validationSourcesController,
                null, // uriRedirectController
                destinationController,
                productDownloadsDataController,
                productsDataController,
                accountProductsDataController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
