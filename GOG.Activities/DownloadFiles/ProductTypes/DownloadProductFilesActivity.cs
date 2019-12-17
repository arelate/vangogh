using Interfaces.Controllers.Data;

using Interfaces.Status;

using Models.ProductDownloads;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

using GOG.Models;

namespace GOG.Activities.DownloadProductFiles.ProductTypes
{
    public class DownloadProductFilesActivity : DownloadFilesActivity<Product>
    {
		[Dependencies(
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadManualUrlFileAsyncDelegate,GOG.Delegates",
			"Controllers.Status.StatusController,Controllers")]        
        public DownloadProductFilesActivity(
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
