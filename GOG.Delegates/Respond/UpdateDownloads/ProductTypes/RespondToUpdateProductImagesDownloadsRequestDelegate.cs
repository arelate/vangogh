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
    [RespondsToRequests(Method = "updatedownloads", Collection = "productimages")]
    public class RespondToUpdateProductImagesDownloadsRequestDelegate :
        RespondToUpdateDownloadsRequestDelegate<ProductImage>
    {
        [Dependencies(
            typeof(GOG.Delegates.GetDownloadSources.ProductTypes.GetProductImagesDownloadSourcesAsyncDelegate),
            typeof(GetProductImagesDirectoryDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(GetProductDownloadsByIdAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(CommitProductDownloadsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateProductImagesDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getProductImagesDownloadSourcesAsyncDelegate,
            IGetValueDelegate<string,string> getProductImagesDirectoryDelegate,
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<ProductDownloads, long> getProductDownloadsByIdAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            ICommitAsyncDelegate commitProductDownloadsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getProductImagesDownloadSourcesAsyncDelegate,
                getProductImagesDirectoryDelegate,
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