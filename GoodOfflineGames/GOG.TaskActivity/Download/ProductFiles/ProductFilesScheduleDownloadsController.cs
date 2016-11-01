using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.Reporting;
using Interfaces.UriRedirection;
using Interfaces.Destination;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductFiles
{
    public class ProductFilesScheduleDownloadsController: ScheduleDownloadsController
    {
        public ProductFilesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IDestinationController destinationController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base (
                Models.Custom.ScheduledDownloadTypes.File,
                downloadSourcesController,
                uriRedirectController,
                destinationController,
                //productTypeStorageController,
                null,
                null,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
