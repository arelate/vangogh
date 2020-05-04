using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using GOG.Delegates.Data.Models;
using Attributes;
using Models.ProductTypes;
using Delegates.Activities;
using Delegates.Data.Models.ProductTypes;
using Delegates.Itemizations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Server.Download
{
    // TODO: productscreenshots or screenshots?
    [RespondsToRequests(Method = "download", Collection = "productscreenshots")]
    public class
        DownloadProductScreenshotsAsyncDelegate : DownloadAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(ItemizeAllProductDownloadsAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(DeleteProductDownloadsAsyncDelegate),
            typeof(GetProductImageAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public DownloadProductScreenshotsAsyncDelegate(
            IItemizeAllAsyncDelegate<ProductDownloads> itemizeAllProductDownloadsAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            IDeleteAsyncDelegate<ProductDownloads> deleteProductDownloadsAsyncDelegate,
            IGetDataAsyncDelegate<string, ProductFileDownloadManifest> getProductFileAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllProductDownloadsAsyncDelegate,
                updateProductDownloadsAsyncDelegate,
                deleteProductDownloadsAsyncDelegate,
                getProductFileAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}