using Interfaces.Controllers.Data;

using Interfaces.Status;

using Models.ProductDownloads;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

using GOG.Models;
using Models.ProductScreenshots;

namespace GOG.Activities.DownloadProductFiles.ProductTypes
{
    public class DownloadProductScreenshotsActivity : DownloadFilesActivity<ProductScreenshots>
    {
		[Dependencies(
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadProductImageAsyncDelegate,GOG.Delegates",
			"Controllers.Status.StatusController,Controllers")]        
        public DownloadProductScreenshotsActivity(
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
