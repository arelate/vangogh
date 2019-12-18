using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

namespace GOG.Activities.DownloadProductFiles.ProductTypes
{
    public class DownloadProductImagesActivity : DownloadFilesActivity<ProductImage>
    {
		[Dependencies(
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadProductImageAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ResponseLogController,Controllers")]
        public DownloadProductImagesActivity(
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
