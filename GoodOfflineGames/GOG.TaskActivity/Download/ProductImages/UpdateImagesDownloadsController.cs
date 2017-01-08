using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductDownloads;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductImages
{
    public class UpdateImagesDownloadsController : UpdateDownloadsController
    {
        public UpdateImagesDownloadsController(
            ProductDownloadTypes downloadType,
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                downloadType,
                downloadSourcesController,
                destinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
