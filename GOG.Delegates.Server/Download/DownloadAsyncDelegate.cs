using System.Collections.Generic;
using System.Threading.Tasks;
using GOG.Models;
using Interfaces.Delegates.Server;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Itemizations;
using Models.ProductTypes;
using Models.Separators;

namespace GOG.Delegates.Server.Download
{
    public abstract class DownloadAsyncDelegate<Type> : IProcessAsyncDelegate
        where Type : ProductCore
    {
        private readonly IItemizeAllAsyncDelegate<ProductDownloads> itemizeAllProductDownloadsAsyncDelegate;
        private readonly IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate;
        private readonly IDeleteAsyncDelegate<ProductDownloads> deleteProductDownloadsAsyncDelegate;
        private readonly IGetDataAsyncDelegate<string, ProductFileDownloadManifest> getProductFileAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public DownloadAsyncDelegate(
            IItemizeAllAsyncDelegate<ProductDownloads> itemizeAllProductDownloadsAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            IDeleteAsyncDelegate<ProductDownloads> deleteProductDownloadsAsyncDelegate,
            IGetDataAsyncDelegate<string, ProductFileDownloadManifest> getProductFileAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.itemizeAllProductDownloadsAsyncDelegate = itemizeAllProductDownloadsAsyncDelegate;
            this.updateProductDownloadsAsyncDelegate = updateProductDownloadsAsyncDelegate;
            this.deleteProductDownloadsAsyncDelegate = deleteProductDownloadsAsyncDelegate;
            this.getProductFileAsyncDelegate = getProductFileAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task ProcessAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start(
                $"Process updated {typeof(Type)} downloads");

            var emptyProductDownloads = new List<ProductDownloads>();

            await foreach (var productDownloads in itemizeAllProductDownloadsAsyncDelegate.ItemizeAllAsync())
            {
                if (productDownloads == null) continue;

                // we'll need to remove successfully downloaded files, copying collection
                var downloadEntries = productDownloads.Downloads.FindAll(
                    d =>
                        d.Type == typeof(Type).ToString()).ToArray();

                startDelegate.Start($"Download {typeof(Type)} entries");

                foreach (var entry in downloadEntries)
                {
                    var sanitizedUri = entry.SourceUri;
                    if (sanitizedUri.Contains(Separators.QueryString))
                        sanitizedUri = sanitizedUri.Substring(0,
                            sanitizedUri.IndexOf(Separators.QueryString, System.StringComparison.Ordinal));

                    setProgressDelegate.SetProgress();

                    if (getProductFileAsyncDelegate != null)
                        await getProductFileAsyncDelegate.GetDataAsync(
                            new ProductFileDownloadManifest(
                                productDownloads.Id,
                                productDownloads.Title,
                                sanitizedUri,
                                entry.Destination));

                    startDelegate.Start($"Remove scheduled {typeof(Type)} downloaded entry");

                    productDownloads.Downloads.Remove(entry);
                    await updateProductDownloadsAsyncDelegate.UpdateAsync(productDownloads);

                    completeDelegate.Complete();
                }

                // if there are no scheduled downloads left - mark file for removal
                if (productDownloads.Downloads.Count == 0)
                    emptyProductDownloads.Add(productDownloads);

                completeDelegate.Complete();
            }

            startDelegate.Start("Clear empty downloads");

            foreach (var productDownload in emptyProductDownloads)
                await deleteProductDownloadsAsyncDelegate.DeleteAsync(productDownload);

            completeDelegate.Complete();

            completeDelegate.Complete();
        }
    }
}