using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.Reporting;
using Interfaces.Destination;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Validation
{
    public class ValidationScheduleDownloadsController: ScheduleDownloadsController
    {
        public ValidationScheduleDownloadsController(
            IDownloadSourcesController validationSourcesController,
            IDestinationController destinationController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base (
                Models.Custom.ScheduledDownloadTypes.Validation,
                validationSourcesController,
                null, // uriRedirectController
                destinationController,
                productTypeStorageController,
                collectionController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
