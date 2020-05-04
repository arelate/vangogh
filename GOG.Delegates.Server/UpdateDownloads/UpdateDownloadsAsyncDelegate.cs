using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;
using Interfaces.Delegates.Server;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

// TODO: Should this be just update if collections don't overlap? e.g. update accountproductimages vs updatedownloads accountproductimages
namespace GOG.Delegates.Server.UpdateDownloads
{
    public abstract class UpdateDownloadsAsyncDelegate<Type> : IProcessAsyncDelegate
        where Type : ProductCore
    {
        private readonly IItemizeAllAsyncDelegate<(long Id, IList<string> Downloads)> itemizeAllDownloadSourcesAsyncDelegate;
        private readonly IGetValueDelegate<string,string> getDirectoryDelegate;
        private readonly IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate;
        private readonly IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate;
        private readonly IGetDataAsyncDelegate<ProductDownloads, long> getProductDownloadsByIdAsyncDelegate;
        private readonly IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate;
        private readonly ICommitAsyncDelegate commitProductDownloadsAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public UpdateDownloadsAsyncDelegate(
            IItemizeAllAsyncDelegate<(long, IList<string>)> itemizeAllDownloadSourcesAsyncDelegate,
            IGetValueDelegate<string,string> getDirectoryDelegate,
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<ProductDownloads, long> getProductDownloadsByIdAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            ICommitAsyncDelegate commitProductDownloadsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllDownloadSourcesAsyncDelegate = itemizeAllDownloadSourcesAsyncDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getProductByIdAsyncDelegate = getProductByIdAsyncDelegate;
            this.getAccountProductByIdAsyncDelegate = getAccountProductByIdAsyncDelegate;
            this.getProductDownloadsByIdAsyncDelegate = getProductDownloadsByIdAsyncDelegate;
            this.updateProductDownloadsAsyncDelegate = updateProductDownloadsAsyncDelegate;
            this.commitProductDownloadsAsyncDelegate = commitProductDownloadsAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task ProcessAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start(
                $"Update {typeof(Type)} downloads");

            startDelegate.Start($"Get {typeof(Type)} download sources");
            var downloadSources = itemizeAllDownloadSourcesAsyncDelegate.ItemizeAllAsync();
            completeDelegate.Complete();

            startDelegate.Start("Update individual downloads");
            await foreach (var downloadSource in downloadSources)
            {
                // don't perform expensive updates if there are no actual sources
                var id = downloadSource.Id;

                ProductCore product = await getProductByIdAsyncDelegate.GetDataAsync(id);

                if (product == null)
                {
                    product = await getAccountProductByIdAsyncDelegate.GetDataAsync(id);

                    if (product == null)
                        // await statusController.WarnAsync(
                        //     updateDownloadsTask,
                        //     $"Downloads are scheduled for the product/account product {id} that doesn't exist");
                        continue;
                }

                setProgressDelegate.SetProgress();

                var productDownloads = await getProductDownloadsByIdAsyncDelegate.GetDataAsync(product.Id);
                if (productDownloads == null)
                    productDownloads = new ProductDownloads
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Downloads = new List<ProductDownloadEntry>()
                    };

                // purge existing downloads for this download type as we'll always be scheduling all files we need to download
                // and don't want to carry over any previously scheduled files that might not be relevant anymore
                // (e.g. files that were scheduled, but never downloaded and then removed from data files)
                var existingDownloadsOfType = productDownloads.Downloads.FindAll(
                    d => d.Type == typeof(Type).ToString()).ToArray();
                foreach (var download in existingDownloadsOfType)
                    productDownloads.Downloads.Remove(download);

                startDelegate.Start("Schedule new downloads");

                foreach (var source in downloadSource.Downloads)
                {
                    var destinationDirectory = getDirectoryDelegate?.GetValue(source);

                    var scheduledDownloadEntry = new ProductDownloadEntry
                    {
                        Type = typeof(Type).ToString(),
                        SourceUri = source,
                        Destination = destinationDirectory
                    };

                    var destinationUri = Path.Combine(
                        destinationDirectory,
                        Path.GetFileName(source));

                    // we won't schedule downloads for the already existing files
                    // we won't be able to resolve filename for productFiles, but that should cut off 
                    // number of images we constantly try to redownload
                    if (File.Exists(destinationUri)) continue;

                    productDownloads.Downloads.Add(scheduledDownloadEntry);
                }

                await updateProductDownloadsAsyncDelegate.UpdateAsync(productDownloads);

                completeDelegate.Complete();
            }

            await commitProductDownloadsAsyncDelegate.CommitAsync();
            
            completeDelegate.Complete();

            completeDelegate.Complete();
        }
    }
}