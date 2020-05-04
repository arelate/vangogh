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
    [RespondsToRequests(Method = "download", Collection = "productfiles")]
    public class DownloadProductFilesAsyncDelegate : DownloadAsyncDelegate<ProductFile>
    {
        [Dependencies(
            typeof(ItemizeAllProductDownloadsAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(DeleteProductDownloadsAsyncDelegate),
            typeof(GetManualUrlFileAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public DownloadProductFilesAsyncDelegate(
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