using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductDownloads;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.Validation
{
    public class UpdateValidationDownloadsController: UpdateDownloadsController
    {
        public UpdateValidationDownloadsController(
            ProductDownloadTypes productDownloadType,
            IDownloadSourcesController downloadSourcesController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(productDownloadType,
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
