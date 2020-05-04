using System.Collections.Generic;
using Attributes;
using Delegates.Activities;
using Delegates.Data.Models.ProductTypes;
using Delegates.Values.Directories.ProductTypes;
using GOG.Delegates.Itemizations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace GOG.Delegates.Server.UpdateDownloads
{
    [RespondsToRequests(Method = "updatedownloads", Collection = "accountproductimages")]
    public class UpdateAccountProductImagesDownloadsAsyncDelegate :
        UpdateDownloadsAsyncDelegate<AccountProductImage>
    {
        [Dependencies(
            typeof(ItemizeAllAccountProductImagesDownloadSourcesAsyncDelegate),
            typeof(GetAccountProductImagesDirectoryDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(GetProductDownloadsByIdAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(CommitProductDownloadsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public UpdateAccountProductImagesDownloadsAsyncDelegate(
            IItemizeAllAsyncDelegate<(long, IList<string>)> itemizeAccountProductImagesDownloadSourcesAsyncDelegate,
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
                itemizeAccountProductImagesDownloadSourcesAsyncDelegate,
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