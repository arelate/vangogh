using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Models.ProductTypes;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

namespace GOG.Delegates.Respond.Download.ProductTypes
{
    [RespondsToRequests(Method = "download", Collection = "productimages")]
    public class RespondToDownloadProductImagesRequestDelegate : RespondToDownloadRequestDelegate<ProductImage>
    {
		[Dependencies(
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadProductImageAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]
        public RespondToDownloadProductImagesRequestDelegate(
            IDataController<ProductDownloads> productDownloadsDataController,
            IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate,
            IActionLogController actionLogController) :
            base(
                productDownloadsDataController,
                downloadProductFileAsyncDelegate,
                actionLogController)
        {
            // ...
        }
    }
}
