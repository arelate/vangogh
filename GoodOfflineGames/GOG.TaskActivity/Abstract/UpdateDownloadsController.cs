using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Reporting;
using Interfaces.DownloadSources;
using Interfaces.Destination;
using Interfaces.Data;

using Models.ProductCore;

using GOG.Models;
using GOG.Models.Custom;

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
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
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
            taskReportingController.StartTask(
                "Get sources for the type: {0}", 
                scheduledDownloadTitle);

            var downloadSources = await downloadSourcesController.GetDownloadSourcesAsync();
            taskReportingController.CompleteTask();

            foreach (var downloadSource in downloadSources)
            {
                var key = downloadSource.Key;

                ProductCore product = await accountProductsDataController.GetByIdAsync(key);

                if (product == null)
                {
                    taskReportingController.ReportWarning(
                        "Downloads are scheduled for the product/account product that doesn't exist");
                    continue;
                }

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

                    taskReportingController.StartTask(
                        "Schedule new downloads for: {0}, {1}",
                        scheduledDownloadTitle,
                        product.Title);

                    var scheduledDownloadEntry = new ProductDownloadEntry()
                    {
                        Type = downloadType,
                        SourceUri = source,
                        Destination = destinationDirectory
                    };
                    productDownloads.Downloads.Add(scheduledDownloadEntry);

                    await productDownloadsDataController.UpdateAsync(productDownloads);

                    taskReportingController.CompleteTask();
                }
            }
        }
    }
}
