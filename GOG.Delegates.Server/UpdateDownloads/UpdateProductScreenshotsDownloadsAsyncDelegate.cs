using System.Collections.Generic;
using Attributes;
using Delegates.Activities;
using Delegates.Data.Models.ProductTypes;
using Delegates.Values.Directories.ProductTypes;
using GOG.Delegates.Itemizations;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace GOG.Delegates.Server.UpdateDownloads
{
    // TODO: productscreenshots or just screenshots?
    [RespondsToRequests(Method = "updatedownloads", Collection = "productscreenshots")]
    public class UpdateProductScreenshotsDownloadsAsyncDelegate :
        UpdateDownloadsAsyncDelegate<ProductScreenshots>
    {
        [Dependencies(
            typeof(ItemizeAllScreenshotsDownloadSourcesAsyncDelegate),
            typeof(GetScreenshotsDirectoryDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(GetProductDownloadsByIdAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(CommitProductDownloadsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public UpdateProductScreenshotsDownloadsAsyncDelegate(
            IItemizeAllAsyncDelegate<(long, IList<string>)> itemizeAllProductScreenshotsDownloadSourcesAsyncDelegate,
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
                itemizeAllProductScreenshotsDownloadSourcesAsyncDelegate,
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