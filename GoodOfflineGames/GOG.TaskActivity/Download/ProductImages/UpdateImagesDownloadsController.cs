using Interfaces.Reporting;
using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;

using Models.ProductDownloads;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.ProductImages
{
    public class UpdateImagesDownloadsController : UpdateDownloadsController
    {
        public UpdateImagesDownloadsController(
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskReportingController taskReportingController) :
            base(
                ProductDownloadTypes.Image,
                downloadSourcesController, 
                destinationController,
                productDownloadsDataController,
                accountProductsDataController,
                taskReportingController)
        {
            // ...
        }
    }
}
