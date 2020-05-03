using Interfaces.Delegates.Values;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.GetDownloadSources;
using Attributes;
using GOG.Models;
using Models.ProductTypes;
using Delegates.Data.Models.ProductTypes;
using Delegates.Activities;
using Delegates.Values.Directories.ProductTypes;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    // TODO: productscreenshots or just screenshots?
    [RespondsToRequests(Method = "updatedownloads", Collection = "productscreenshots")]
    public class RespondToUpdateProductScreenshotsDownloadsRequestDelegate :
        RespondToUpdateDownloadsRequestDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(GetDownloadSources.GetScreenshotsDownloadSourcesAsyncDelegate),
            typeof(GetScreenshotsDirectoryDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(GetProductDownloadsByIdAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(CommitProductDownloadsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateProductScreenshotsDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getProductScreenshotsDownloadSourcesAsyncDelegate,
            IGetValueDelegate<string,string> getProductScreenshotsDirectoryDelegate,
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