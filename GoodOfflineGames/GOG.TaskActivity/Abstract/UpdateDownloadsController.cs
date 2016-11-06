using System.Collections.Generic;
using System.Threading.Tasks;

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

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask(
                "Get sources for the type: {0}", 
                scheduledDownloadTitle);

            var downloadSources = await downloadSourcesController.GetDownloadSources();
            taskReportingController.CompleteTask();

            foreach (var downloadSource in downloadSources)
            {
                var key = downloadSource.Key;

                ProductCore product = await accountProductsDataController.GetById(key);

                if (product == null)
                {
                    taskReportingController.ReportWarning(
                        "Downloads are scheduled for the product/account product that doesn't exist");
                    continue;
                }

                var productDownloads = await productDownloadsDataController.GetById(product.Id);
                if (productDownloads == null)
                {
                    productDownloads = new ProductDownloads();
                    productDownloads.Id = product.Id;
                    productDownloads.Title = product.Title;
                    productDownloads.Downloads = new List<ProductDownloadEntry>();
                }

                var productSourceAlreadyScheduled = false;

                foreach (var source in downloadSource.Value)
                {
                    // skip the source if we've already scheduled a download for same id
                    foreach (var download in productDownloads.Downloads)
                        if (download.SourceUri == source) productSourceAlreadyScheduled = true;

                    if (productSourceAlreadyScheduled) continue;

                    var destinationDirectory = destinationController?.GetDirectory(source);

                    taskReportingController.StartTask(
                        "Schedule new downloads for: {0}, {1}",
                        scheduledDownloadTitle,
                        product.Title);

                    var scheduledDownloadEntry = new ProductDownloadEntry();
                    scheduledDownloadEntry.Type = downloadType;
                    scheduledDownloadEntry.SourceUri = source;
                    scheduledDownloadEntry.Destination = destinationDirectory;

                    productDownloads.Downloads.Add(scheduledDownloadEntry);

                    await productDownloadsDataController.Update(productDownloads);

                    taskReportingController.CompleteTask();
                }
            }
        }
    }
}
