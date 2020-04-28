using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using GOG.Interfaces.Delegates.GetDownloadSources;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Respond.UpdateDownloads.ProductTypes
{
    [RespondsToRequests(Method = "updatedownloads", Collection = "productfiles")]
    public class RespondToUpdateProductFilesDownloadsRequestDelegate :
        RespondToUpdateDownloadsRequestDelegate<ProductFile>
    {
        [Dependencies(
            "GOG.Delegates.GetDownloadSources.GetManualUrlDownloadSourcesAsyncDelegate,GOG.Delegates",
            "Delegates.GetDirectory.ProductTypes.GetProductFilesDirectoryDelegate,Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate,GOG.Delegates",
            "Delegates.Data.Models.ProductTypes.GetProductDownloadsByIdAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.UpdateProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.CommitProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToUpdateProductFilesDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getProductFilesDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getProductFilesDirectoryDelegate,
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