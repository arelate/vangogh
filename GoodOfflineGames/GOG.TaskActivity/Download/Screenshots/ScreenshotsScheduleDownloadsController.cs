using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
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
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
