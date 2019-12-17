using Interfaces.Controllers.Data;

using Interfaces.Status;

using Models.ProductDownloads;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

using GOG.Models;

namespace GOG.Activities.DownloadProductFiles.ProductTypes
{
    public class DownloadProductImagesActivity : DownloadFilesActivity<Product>
    {
        public DownloadProductImagesActivity(
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate,
            IStatusController statusController) :
            base(
                productDownloadsDataController,
                downloadProductFileAsyncDelegate,
                statusController)
        {
            // ...
        }
    }
}
