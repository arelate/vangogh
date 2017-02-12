using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Destination.Directory;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.ProductCore;
using Models.ProductDownloads;

using GOG.Models;

namespace GOG.TaskActivities.Download
{
    public abstract class UpdateDownloadsController : TaskActivityController
    {
        private ProductDownloadTypes downloadType;

        private IDownloadSourcesController downloadSourcesController;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<AccountProduct> accountProductsDataController;

        private string scheduledDownloadTitle;

        public UpdateDownloadsController(
            ProductDownloadTypes downloadType,
            IDownloadSourcesController downloadSourcesController,
            IGetDirectoryDelegate getDirectoryDelegate,
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
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.productDownloadsDataController = productDownloadsDataController;
            this.accountProductsDataController = accountProductsDataController;

            scheduledDownloadTitle = System.Enum.GetName(typeof(ProductDownloadTypes), downloadType);
        }

        public override async Task ProcessTaskAsync()
        {
            var updateDownloadsTask = taskStatusController.Create(
                taskStatus,
                string.Format(
                    "Update downloads for the type: {0}",
                    scheduledDownloadTitle));

            var getSourcesTask = taskStatusController.Create(
                updateDownloadsTask,
                string.Format(
                    "Get download sources",
                    scheduledDownloadTitle));

            var downloadSources = await downloadSourcesController.GetDownloadSourcesAsync(getSourcesTask);
            taskStatusController.Complete(getSourcesTask);

            var counter = 0;

            foreach (var downloadSource in downloadSources)
            {
                var id = downloadSource.Key;

                ProductCore product = await accountProductsDataController.GetByIdAsync(id);

                if (product == null)
                {
                    taskStatusController.Warn(
                        updateDownloadsTask,
                        "Downloads are scheduled for the product/account product that doesn't exist: {0}",
                        id);
                    continue;
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
                var existingDownloadsOfType = productDownloads.Downloads.FindAll(d => d.Type == downloadType).ToArray();
                foreach (var download in existingDownloadsOfType)
                    productDownloads.Downloads.Remove(download);

                foreach (var source in downloadSource.Value)
                {
                    var destinationDirectory = getDirectoryDelegate?.GetDirectory(source);

                    var scheduleDownloadsTask = taskStatusController.Create(
                        updateDownloadsTask,
                        "Schedule new downloads");

                    var scheduledDownloadEntry = new ProductDownloadEntry()
                    {
                        Type = downloadType,
                        SourceUri = source,
                        Destination = destinationDirectory
                    };
                    productDownloads.Downloads.Add(scheduledDownloadEntry);

                    await productDownloadsDataController.UpdateAsync(scheduleDownloadsTask, productDownloads);

                    taskStatusController.Complete(scheduleDownloadsTask);
                }
            }

            taskStatusController.Complete(updateDownloadsTask);
        }
    }
}
