using Interfaces.Delegates.Values;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using GOG.Interfaces.Delegates.GetDownloadSources;
using Attributes;
using GOG.Models;
using Delegates.Data.Models.ProductTypes;
using Delegates.Activities;
using Delegates.Values.Directories.ProductTypes;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    [RespondsToRequests(Method = "updatedownloads", Collection = "productfiles")]
    public class RespondToUpdateProductFilesDownloadsRequestDelegate :
        RespondToUpdateDownloadsRequestDelegate<ProductFile>
    {
        [Dependencies(
            typeof(GetDownloadSources.GetManualUrlDownloadSourcesAsyncDelegate),
            typeof(GetProductFilesDirectoryDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(GetProductDownloadsByIdAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(CommitProductDownloadsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateProductFilesDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getProductFilesDownloadSourcesAsyncDelegate,
            IGetValueDelegate<string,string> getProductFilesDirectoryDelegate,
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<ProductDownloads, long> getProductDownloadsByIdAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            ICommitAsyncDelegate commitProductDownloadsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getProductFilesDownloadSourcesAsyncDelegate,
                getProductFilesDirectoryDelegate,
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