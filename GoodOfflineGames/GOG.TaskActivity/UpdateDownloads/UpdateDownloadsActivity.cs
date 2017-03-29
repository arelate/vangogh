using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Interfaces.DownloadSources;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.Data;
using Interfaces.TaskStatus;
using Interfaces.File;

using Models.ProductCore;
using Models.ProductDownloads;

using GOG.Models;

namespace GOG.TaskActivities.UpdateDownloads
{
    public class UpdateDownloadsActivity : TaskActivityController
    {
        private string downloadParameter;

        private IDownloadSourcesController downloadSourcesController;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IFileController fileController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<AccountProduct> accountProductsDataController;
        private IDataController<Product> productsDataController;

        public UpdateDownloadsActivity(
            string downloadParameter,
            IDownloadSourcesController downloadSourcesController,
            IGetDirectoryDelegate getDirectoryDelegate,
            IFileController fileController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<Product> productsDataController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.downloadParameter = downloadParameter;
            this.downloadSourcesController = downloadSourcesController;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.fileController = fileController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.accountProductsDataController = accountProductsDataController;
            this.productsDataController = productsDataController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var updateDownloadsTask = taskStatusController.Create(
                taskStatus,
                $"Update {downloadParameter} downloads");

            var getSourcesTask = taskStatusController.Create(
                updateDownloadsTask,
                $"Get {downloadParameter} download sources");

            var downloadSources = await downloadSourcesController.GetDownloadSourcesAsync(getSourcesTask);
            taskStatusController.Complete(getSourcesTask);

            var counter = 0;

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
                        taskStatusController.Warn(
                            updateDownloadsTask,
                            $"Downloads are scheduled for the product/account product {id} that doesn't exist");
                        continue;
                    }
                }

                taskStatusController.UpdateProgress(
                    updateDownloadsTask, 
                    ++counter, 
                    downloadSources.Count, 
                    product.Title);

                var productDownloads = await productDownloadsDataController.GetByIdAsync(product.Id);
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
                var existingDownloadsOfType = productDownloads.Downloads.FindAll(d => d.DownloadParameter == downloadParameter).ToArray();
                foreach (var download in existingDownloadsOfType)
                    productDownloads.Downloads.Remove(download);

                var scheduleDownloadsTask = taskStatusController.Create(
                    updateDownloadsTask,
                    "Schedule new downloads");

                foreach (var source in downloadSource.Value)
                {
                    var destinationDirectory = getDirectoryDelegate?.GetDirectory(source);

                    var scheduledDownloadEntry = new ProductDownloadEntry()
                    {
                        DownloadParameter = downloadParameter,
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

                taskStatusController.Complete(scheduleDownloadsTask);
            }

            taskStatusController.Complete(updateDownloadsTask);
        }
    }
}
