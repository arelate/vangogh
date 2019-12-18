using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

using Models.ProductTypes;

namespace GOG.Activities.DownloadProductFiles.ProductTypes
{
    public class DownloadProductScreenshotsActivity : DownloadFilesActivity<ProductScreenshots>
    {
		[Dependencies(
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadProductImageAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ResponseLogController,Controllers")]        
        public DownloadProductScreenshotsActivity(
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate,
            IResponseLogController responseLogController) :
            base(
                productDownloadsDataController,
                downloadProductFileAsyncDelegate,
                responseLogController)
        {
            // ...
        }
    }
}
