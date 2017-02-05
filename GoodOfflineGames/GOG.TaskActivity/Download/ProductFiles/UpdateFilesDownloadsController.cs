using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductDownloads;

using GOG.Models;

using GOG.TaskActivities.Abstract;

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
