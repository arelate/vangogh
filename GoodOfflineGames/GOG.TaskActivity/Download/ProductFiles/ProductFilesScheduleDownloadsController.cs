using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.Reporting;
using Interfaces.UriRedirect;
using Interfaces.GOGUri;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductFiles
{
    public class ProductFilesScheduleDownloadsController: ScheduleDownloadsController
    {
        public ProductFilesScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IGOGUriController gogUriController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base (
                Models.Custom.ScheduledDownloadTypes.File,
                string.Empty,
                downloadSourcesController,
                uriRedirectController,
                gogUriController,
                productTypeStorageController,
                collectionController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
