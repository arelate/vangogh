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
    [RespondsToRequests(Method = "updatedownloads", Collection = "productimages")]
    public class UpdateProductImagesDownloadsAsyncDelegate :
        UpdateDownloadsAsyncDelegate<ProductImage>
    {
        [Dependencies(
            typeof(ItemizeAllProductImagesDownloadSourcesAsyncDelegate),
            typeof(GetProductImagesDirectoryDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate),
            typeof(GetProductDownloadsByIdAsyncDelegate),
            typeof(UpdateProductDownloadsAsyncDelegate),
            typeof(CommitProductDownloadsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public UpdateProductImagesDownloadsAsyncDelegate(
            IItemizeAllAsyncDelegate<(long, IList<string>)> itemizeAllProductImagesDownloadSourcesAsyncDelegate,
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
                itemizeAllProductImagesDownloadSourcesAsyncDelegate,
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