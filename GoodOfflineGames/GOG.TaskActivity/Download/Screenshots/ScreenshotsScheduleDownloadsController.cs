using Interfaces.Reporting;
using Interfaces.File;
using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;

using GOG.Models;
using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Screenshots
{
    public class ScreenshotsScheduleDownloadsController : ScheduleDownloadsController
    {
        public ScreenshotsScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ScheduledDownload> scheduledDownloadsDataController,
            IDataController<Product> productsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                ScheduledDownloadTypes.Screenshot,
                downloadSourcesController,
                null, // uriRedirectionController
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
