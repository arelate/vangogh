using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;


using Models.ProductTypes;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Respond.Download.ProductTypes
{
    [RespondsToRequests(Method = "download", Collection = "accountproductimages")]
    public class RespondToDownloadAccountProductImagesRequestDelegate : RespondToDownloadRequestDelegate<AccountProductImage>
    {
		[Dependencies(
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadProductImageAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]        
        public RespondToDownloadAccountProductImagesRequestDelegate(
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
