using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.File;

using Interfaces.Status;
using Interfaces.ContextDefinitions;

using Models.ProductCore;
using Models.ProductDownloads;

using GOG.Interfaces.Delegates.GetDownloadSources;

using GOG.Models;

namespace GOG.Activities.UpdateDownloads
{
    public class UpdateDownloadsActivity : Activity
    {
        private Context context;

        private IGetDownloadSourcesAsyncDelegate getDownloadSourcesAsyncDelegate;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IFileController fileController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<AccountProduct> accountProductsDataController;
        private IDataController<Product> productsDataController;

        public UpdateDownloadsActivity(
            Context context,
            IGetDownloadSourcesAsyncDelegate getDownloadSourcesAsyncDelegate,
            IGetDirectoryDelegate getDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            IStatusController statusController) :
            base(statusController)
        {
            this.context = context;
            this.getDownloadSourcesAsyncDelegate = getDownloadSourcesAsyncDelegate;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.fileController = fileController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.accountProductsDataController = accountProductsDataController;
            this.productsDataController = productsDataController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateDownloadsTask = await statusController.CreateAsync(
                status,
                $"Update {context} downloads");

            var getSourcesTask = await statusController.CreateAsync(
                updateDownloadsTask,
                $"Get {context} download sources");

            var downloadSources = await getDownloadSourcesAsyncDelegate.GetDownloadSourcesAsync(getSourcesTask);
            await statusController.CompleteAsync(getSourcesTask);

            var counter = 0;

            foreach (var downloadSource in downloadSources)
            {
                // don't perform expensive updates if there are no actual sources
                if (downloadSource.Value != null &&
                    downloadSource.Value.Count == 0) continue;

                var id = downloadSource.Key;

                ProductCore product = await productsDataController.GetByIdAsync(id, updateDownloadsTask);

                if (product == null)
                {
                    product = await accountProductsDataController.GetByIdAsync(id, updateDownloadsTask);

                    if (product == null)
                    {
                        await statusController.WarnAsync(
                            updateDownloadsTask,
                            $"Downloads are scheduled for the product/account product {id} that doesn't exist");
                        continue;
                    }
                }

                await statusController.UpdateProgressAsync(
                    updateDownloadsTask,
                    ++counter,
                    downloadSources.Count,
                    product.Title);

                var productDownloads = await productDownloadsDataController.GetByIdAsync(product.Id, updateDownloadsTask);
                if (productDownloads == null)
                {
                    productDownloads = new ProductDownloads()
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
                    d => d.Context == context).ToArray();
                foreach (var download in existingDownloadsOfType)
                    productDownloads.Downloads.Remove(download);

                var scheduleDownloadsTask = await statusController.CreateAsync(
                    updateDownloadsTask,
                    "Schedule new downloads");

                foreach (var source in downloadSource.Value)
                {
                    var destinationDirectory = getDirectoryDelegate?.GetDirectory(source);

                    var scheduledDownloadEntry = new ProductDownloadEntry()
                    {
                        Context = context,
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

                await productDownloadsDataController.UpdateAsync(scheduleDownloadsTask, productDownloads);

                await statusController.CompleteAsync(scheduleDownloadsTask);
            }

            await statusController.CompleteAsync(updateDownloadsTask);
        }
    }
}
