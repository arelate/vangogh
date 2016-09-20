using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.Reporting;
using Interfaces.UriRedirect;
using Interfaces.GOGUri;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductExtras
{
    public class ProductExtrasScheduleDownloadsController : ScheduleDownloadsController
    {
        public ProductExtrasScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IGOGUriController gogUriController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base (
                Models.Custom.ScheduledDownloadTypes.Extra,
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
