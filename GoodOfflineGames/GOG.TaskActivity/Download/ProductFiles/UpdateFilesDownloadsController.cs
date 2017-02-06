using Interfaces.DownloadSources;
using Interfaces.Destination.Directory;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductDownloads;

using GOG.Models;

namespace GOG.TaskActivities.Download.ProductFiles
{
    public class UpdateFilesDownloadsController: UpdateDownloadsController
    {
        public UpdateFilesDownloadsController(
            ProductDownloadTypes productDownloadType,
            IDownloadSourcesController downloadSourcesController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base (
                productDownloadType,
                downloadSourcesController,
                getDirectoryDelegate,
                productDownloadsDataController,
                accountProductsDataController,
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
