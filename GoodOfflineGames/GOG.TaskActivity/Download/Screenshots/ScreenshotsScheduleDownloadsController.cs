using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.Destination;
using Interfaces.ImageUri;
using Interfaces.DownloadSources;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Screenshots
{
    public class ScreenshotsScheduleDownloadsController : ScheduleDownloadsController
    {
        public ScreenshotsScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IProductTypeStorageController productTypeStorageController,
            IImageUriController screenshotsUriController,
            ICollectionController collectionController,
            IDestinationController destinationController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                downloadSourcesController,
                productTypeStorageController,
                collectionController,
                destinationController,
                fileController,
                taskReportingController)
        {
            downloadType = Models.Custom.ScheduledDownloadTypes.Screenshot;
            destination = "_screenshots";
        }
    }
}
