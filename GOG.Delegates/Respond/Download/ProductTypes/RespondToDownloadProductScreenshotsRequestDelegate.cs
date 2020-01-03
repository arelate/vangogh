using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using GOG.Interfaces.Delegates.DownloadProductFile;

using Attributes;

using Models.ProductTypes;

namespace GOG.Delegates.Respond.Download.ProductTypes
{
    // TODO: productscreenshots or screenshots?
    [RespondsToRequests(Method = "download", Collection = "productscreenshots")]
    public class RespondToDownloadProductScreenshotsRequestDelegate : RespondToDownloadRequestDelegate<ProductScreenshots>
    {
		[Dependencies(
            DependencyContext.Default,
			"Controllers.Data.ProductTypes.ProductDownloadsDataController,Controllers",
			"GOG.Delegates.DownloadProductFile.DownloadProductImageAsyncDelegate,GOG.Delegates",
			"Controllers.Logs.ActionLogController,Controllers")]        
        public RespondToDownloadProductScreenshotsRequestDelegate(
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
