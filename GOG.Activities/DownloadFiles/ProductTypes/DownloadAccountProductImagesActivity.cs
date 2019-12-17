using Interfaces.Controllers.Data;

using Interfaces.Status;

using Models.ProductDownloads;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

using GOG.Models;

namespace GOG.Activities.DownloadProductFiles.ProductTypes
{
    public class DownloadAccountProductImagesActivity : DownloadFilesActivity<AccountProduct>
    {
		[Dependencies(
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadProductImageAsyncDelegate,GOG.Delegates",
			"Controllers.Status.StatusController,Controllers")]        
        public DownloadAccountProductImagesActivity(
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
