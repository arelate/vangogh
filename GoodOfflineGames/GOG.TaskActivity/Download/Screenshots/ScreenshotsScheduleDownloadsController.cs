using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.SourceDestination;
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
            ISourceDestinationController sourceDestinationController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(
                Models.Custom.ScheduledDownloadTypes.Screenshot,
                "_screenshots",
                downloadSourcesController,
                null, // uriResolutionController
                null, // destinationAdjustmentController
                productTypeStorageController,
                collectionController,
                sourceDestinationController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
