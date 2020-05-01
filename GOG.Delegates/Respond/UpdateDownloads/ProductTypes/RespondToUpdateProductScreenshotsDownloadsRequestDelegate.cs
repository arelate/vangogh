using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.GetDownloadSources;
using Attributes;
using GOG.Models;
using Models.ProductTypes;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    // TODO: productscreenshots or just screenshots?
    [RespondsToRequests(Method = "updatedownloads", Collection = "productscreenshots")]
    public class RespondToUpdateProductScreenshotsDownloadsRequestDelegate :
        RespondToUpdateDownloadsRequestDelegate<ProductScreenshots>
    {
        [Dependencies(
            "GOG.Delegates.GetDownloadSources.GetScreenshotsDownloadSourcesAsyncDelegate,GOG.Delegates",
            "Delegates.GetDirectory.ProductTypes.GetScreenshotsDirectoryDelegate,Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate,GOG.Delegates",
            "Delegates.Data.Models.ProductTypes.GetProductDownloadsByIdAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.UpdateProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.CommitProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToUpdateProductScreenshotsDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getProductScreenshotsDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getProductScreenshotsDirectoryDelegate,
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<ProductDownloads, long> getProductDownloadsByIdAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            ICommitAsyncDelegate commitProductDownloadsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getProductScreenshotsDownloadSourcesAsyncDelegate,
                getProductScreenshotsDirectoryDelegate,
                getProductByIdAsyncDelegate,
                getAccountProductByIdAsyncDelegate,
                getProductDownloadsByIdAsyncDelegate,
                updateProductDownloadsAsyncDelegate,
                commitProductDownloadsAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}