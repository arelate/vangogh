using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductCore;
using Models.ProductDownloads;

using GOG.Models;

namespace GOG.TaskActivities.Abstract
{
    public abstract class UpdateDownloadsController : TaskActivityController
    {
        private ProductDownloadTypes downloadType;

        private IDownloadSourcesController downloadSourcesController;
        private IDestinationController destinationController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<AccountProduct> accountProductsDataController;

        private string scheduledDownloadTitle;

        public UpdateDownloadsController(
            ProductDownloadTypes downloadType,
            IDownloadSourcesController downloadSourcesController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                taskStatus,
                taskStatusController)
        {
            this.downloadType = downloadType;
            this.downloadSourcesController = downloadSourcesController;
            this.destinationController = destinationController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.accountProductsDataController = accountProductsDataController;

            scheduledDownloadTitle = System.Enum.GetName(typeof(ProductDownloadTypes), downloadType);
        }

        public override async Task ProcessTaskAsync()
        {
            var updateAllTask = taskStatusController.Create(
                taskStatus,
                string.Format(
                    "Update downloads for the type: {0}",
                    scheduledDownloadTitle));

            var getSourcesTask = taskStatusController.Create(
                updateAllTask,
                string.Format(
                    "Get download sources",
                    scheduledDownloadTitle));

            var downloadSources = await downloadSourcesController.GetDownloadSourcesAsync(getSourcesTask);
            taskStatusController.Complete(getSourcesTask);

            var counter = 0;
            var updateProductDownloadsTask = taskStatusController.Create(updateAllTask, "Update downloads for product");

            foreach (var downloadSource in downloadSources)
            {
                var id = downloadSource.Key;

                ProductCore product = await accountProductsDataController.GetByIdAsync(id);

                if (product == null)
                {
                    taskStatusController.Warn(
                        updateProductDownloadsTask,
                        "Downloads are scheduled for the product/account product that doesn't exist: {0}",
                        id);
                    continue;
                }

                taskStatusController.UpdateProgress(
                    updateProductDownloadsTask, 
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
                var existingDownloadsOfType = productDownloads.Downloads.FindAll(d => d.Type == downloadType).ToArray();
                foreach (var download in existingDownloadsOfType)
                    productDownloads.Downloads.Remove(download);

                foreach (var source in downloadSource.Value)
                {
                    var destinationDirectory = destinationController?.GetDirectory(source);

                    var scheduleDownloadsTask = taskStatusController.Create(
                        updateProductDownloadsTask,
                        "Schedule new downloads");

                    var scheduledDownloadEntry = new ProductDownloadEntry()
                    {
                        Type = downloadType,
                        SourceUri = source,
                        Destination = destinationDirectory
                    };
                    productDownloads.Downloads.Add(scheduledDownloadEntry);

                    await productDownloadsDataController.UpdateAsync(productDownloads);

                    taskStatusController.Complete(scheduleDownloadsTask);
                }
            }

            taskStatusController.Complete(updateProductDownloadsTask);
            taskStatusController.Complete(updateAllTask);
        }
    }
}
