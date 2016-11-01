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
            IDataController<ScheduledDownload> scheduledDownloadsDataController,
            IDataController<Product> productsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                ScheduledDownloadTypes.Validation,
                validationSourcesController,
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
