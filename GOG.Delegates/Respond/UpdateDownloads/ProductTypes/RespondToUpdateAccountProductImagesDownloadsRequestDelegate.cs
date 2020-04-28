using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using GOG.Interfaces.Delegates.GetDownloadSources;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    [RespondsToRequests(Method = "updatedownloads", Collection = "accountproductimages")]
    public class RespondToUpdateAccountProductImagesDownloadsRequestDelegate :
        RespondToUpdateDownloadsRequestDelegate<AccountProductImage>
    {
        [Dependencies(
            "GOG.Delegates.GetDownloadSources.ProductTypes.GetAccountProductImagesDownloadSourcesAsyncDelegate,GOG.Delegates",
            "Delegates.GetDirectory.ProductTypes.GetAccountProductImagesDirectoryDelegate,Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate,GOG.Delegates",
            "Delegates.Data.Models.ProductTypes.GetProductDownloadsByIdAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.UpdateProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.CommitProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToUpdateAccountProductImagesDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getAccountProductImagesDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getAccountProductImagesDirectoryDelegate,
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