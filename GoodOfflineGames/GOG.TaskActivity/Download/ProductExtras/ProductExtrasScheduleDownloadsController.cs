using Interfaces.DownloadSources;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.Reporting;
using Interfaces.UriRedirection;
using Interfaces.Destination;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductExtras
{
    public class ProductExtrasScheduleDownloadsController : ScheduleDownloadsController
    {
        public ProductExtrasScheduleDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IDestinationController destinationController,
            //IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base (
                Models.Custom.ScheduledDownloadTypes.Extra,
                downloadSourcesController,
                uriRedirectController,
                destinationController,
                //productTypeStorageController,
                collectionController,
                fileController,
                taskReportingController)
        {
            // ...
        }
    }
}
