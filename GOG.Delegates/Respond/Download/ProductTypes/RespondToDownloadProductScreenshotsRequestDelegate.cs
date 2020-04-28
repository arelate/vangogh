using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.DownloadProductFile;
using Attributes;
using Models.ProductTypes;

namespace GOG.Delegates.Respond.Download.ProductTypes
{
    // TODO: productscreenshots or screenshots?
    [RespondsToRequests(Method = "download", Collection = "productscreenshots")]
    public class
        RespondToDownloadProductScreenshotsRequestDelegate : RespondToDownloadRequestDelegate<ProductScreenshots>
    {
        [Dependencies(
            "Delegates.Itemize.ProductTypes.ItemizeAllProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.UpdateProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.DeleteProductDownloadsAsyncDelegate,Delegates",
            "GOG.Delegates.DownloadProductFile.DownloadProductImageAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToDownloadProductScreenshotsRequestDelegate(
            IItemizeAllAsyncDelegate<ProductDownloads> itemizeAllProductDownloadsAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            IDeleteAsyncDelegate<ProductDownloads> deleteProductDownloadsAsyncDelegate,
            IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllProductDownloadsAsyncDelegate,
                updateProductDownloadsAsyncDelegate,
                deleteProductDownloadsAsyncDelegate,
                downloadProductFileAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}