using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using GOG.Delegates.Data.Models;
using Attributes;
using Delegates.Activities;
using Delegates.Data.Models.ProductTypes;
using Delegates.Itemizations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Server.Download
{
    [RespondsToRequests(Method = "download", Collection = "productimages")]
    public class DownloadProductImagesAsyncDelegate : DownloadAsyncDelegate<ProductImage>
    {
        [Dependencies(
            typeof(ItemizeAllProductDownloadsAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(DeleteProductDownloadsAsyncDelegate),
            typeof(GetProductImageAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public DownloadProductImagesAsyncDelegate(
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