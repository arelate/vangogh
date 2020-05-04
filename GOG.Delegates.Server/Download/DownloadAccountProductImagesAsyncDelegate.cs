using Attributes;
using Delegates.Activities;
using Delegates.Data.Models.ProductTypes;
using Delegates.Itemizations.ProductTypes;
using GOG.Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Models.ProductTypes;

namespace GOG.Delegates.Server.Download
{
    [RespondsToRequests(Method = "download", Collection = "accountproductimages")]
    public class
        DownloadAccountProductImagesAsyncDelegate : DownloadAsyncDelegate<AccountProductImage>
    {
        [Dependencies(
            typeof(ItemizeAllProductDownloadsAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(DeleteProductDownloadsAsyncDelegate),
            typeof(GetProductImageAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public DownloadAccountProductImagesAsyncDelegate(
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