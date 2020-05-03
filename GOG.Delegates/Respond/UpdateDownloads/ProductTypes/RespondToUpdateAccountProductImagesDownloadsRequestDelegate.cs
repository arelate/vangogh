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
    [RespondsToRequests(Method = "updatedownloads", Collection = "accountproductimages")]
    public class RespondToUpdateAccountProductImagesDownloadsRequestDelegate :
        RespondToUpdateDownloadsRequestDelegate<AccountProductImage>
    {
        [Dependencies(
            typeof(GOG.Delegates.GetDownloadSources.ProductTypes.GetAccountProductImagesDownloadSourcesAsyncDelegate),
            typeof(GetAccountProductImagesDirectoryDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(GetProductDownloadsByIdAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(CommitProductDownloadsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateAccountProductImagesDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getAccountProductImagesDownloadSourcesAsyncDelegate,
            IGetValueDelegate<string,string> getAccountProductImagesDirectoryDelegate,
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<ProductDownloads, long> getProductDownloadsByIdAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            ICommitAsyncDelegate commitProductDownloadsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getAccountProductImagesDownloadSourcesAsyncDelegate,
                getAccountProductImagesDirectoryDelegate,
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