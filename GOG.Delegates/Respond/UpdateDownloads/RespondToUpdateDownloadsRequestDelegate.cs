using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.Respond;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.File;
using Interfaces.Controllers.Logs;

using GOG.Interfaces.Delegates.GetDownloadSources;

using Models.ProductTypes;
using GOG.Models;

// TODO: Should this be just update if collections don't overlap?
namespace GOG.Delegates.Respond.UpdateDownloads
{
    public abstract class RespondToUpdateDownloadsRequestDelegate<Type> : IRespondAsyncDelegate
        where Type: ProductCore
    {
        readonly IGetDownloadSourcesAsyncDelegate getDownloadSourcesAsyncDelegate;
        readonly IGetDirectoryDelegate getDirectoryDelegate;
        readonly IFileController fileController;
        readonly IDataController<ProductDownloads> productDownloadsDataController;
        readonly IDataController<AccountProduct> accountProductsDataController;
        readonly IDataController<Product> productsDataController;
        readonly IActionLogController actionLogController;

        public RespondToUpdateDownloadsRequestDelegate(
            IGetDownloadSourcesAsyncDelegate getDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IActionLogController actionLogController)
        {
            this.getDownloadSourcesAsyncDelegate = getDownloadSourcesAsyncDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.fileController = fileController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.accountProductsDataController = accountProductsDataController;
            this.productsDataController = productsDataController;
            this.actionLogController = actionLogController;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            actionLogController.StartAction(
                $"Update {typeof(Type)} downloads");

            actionLogController.StartAction($"Get {typeof(Type)} download sources");
            var downloadSources = await getDownloadSourcesAsyncDelegate.GetDownloadSourcesAsync();
            actionLogController.CompleteAction();

            actionLogController.StartAction("Update individual downloads");
            foreach (var downloadSource in downloadSources)
            {
                // don't perform expensive updates if there are no actual sources
                if (downloadSource.Value != null &&
                    downloadSource.Value.Count == 0) continue;

                var id = downloadSource.Key;

                ProductCore product = await productsDataController.GetByIdAsync(id);

                if (product == null)
                {
                    product = await accountProductsDataController.GetByIdAsync(id);

                    if (product == null)
                    {
                        // await statusController.WarnAsync(
                        //     updateDownloadsTask,
                        //     $"Downloads are scheduled for the product/account product {id} that doesn't exist");
                        continue;
                    }
                }

                actionLogController.IncrementActionProgress();

                var productDownloads = await productDownloadsDataController.GetByIdAsync(product.Id);
                if (productDownloads == null)
                {
                    productDownloads = new ProductDownloads
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Downloads = new List<ProductDownloadEntry>()
                    };
                }

                // purge existing downloads for this download type as we'll always be scheduling all files we need to download
                // and don't want to carry over any previously scheduled files that might not be relevant anymore
                // (e.g. files that were scheduled, but never downloaded and then removed from data files)
                var existingDownloadsOfType = productDownloads.Downloads.FindAll(
                    d => d.Type == typeof(Type).ToString()).ToArray();
                foreach (var download in existingDownloadsOfType)
                    productDownloads.Downloads.Remove(download);

                actionLogController.StartAction("Schedule new downloads");

                foreach (var source in downloadSource.Value)
                {
                    var destinationDirectory = getDirectoryDelegate?.GetDirectory(source);

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
                    if (fileController.Exists(destinationUri)) continue;

                    productDownloads.Downloads.Add(scheduledDownloadEntry);
                }

                await productDownloadsDataController.UpdateAsync(productDownloads);

                actionLogController.CompleteAction();
            }
            actionLogController.CompleteAction();

            actionLogController.CompleteAction();
        }
    }
}
