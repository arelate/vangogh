using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.File;
using Interfaces.DownloadSources;
using Interfaces.UriRedirection;
using Interfaces.Destination;
using Interfaces.Data;

using Models.ProductCore;

using GOG.Models;
using GOG.Models.Custom;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ScheduleDownloadsController : TaskActivityController
    {
        private ProductDownloadTypes downloadType;

        private IDownloadSourcesController downloadSourcesController;
        private IUriRedirectController uriRedirectController;
        private IDestinationController destinationController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IDataController<Product> productsDataController;
        private IDataController<AccountProduct> accountProductsDataController;
        private IFileController fileController;

        private string scheduledDownloadTitle;

        public ScheduleDownloadsController(
            ProductDownloadTypes downloadType,
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IDestinationController destinationController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IDataController<Product> productsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.downloadType = downloadType;
            this.downloadSourcesController = downloadSourcesController;
            this.uriRedirectController = uriRedirectController;
            this.destinationController = destinationController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.productsDataController = productsDataController;
            this.accountProductsDataController = accountProductsDataController;
            this.fileController = fileController;

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
                // try Product first
                ProductCore product = await productsDataController.GetById(key);
                // if it doesn't exist it not sold on GOG.com anymore, but since it's updated - it should be available in account products
                if (product == null)
                    product = await accountProductsDataController.GetById(key);

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

                    var actualSource =
                        uriRedirectController != null ?
                        await uriRedirectController.GetUriRedirect(source) :
                        source;

                    var destinationDirectory = destinationController?.GetDirectory(actualSource);

                    if (fileController != null)
                    {
                        var destinationFilename = destinationController?.GetFilename(actualSource);
                        var localFile = System.IO.Path.Combine(destinationDirectory, destinationFilename);
                        if (fileController.Exists(localFile)) continue;
                    }

                    taskReportingController.StartTask(
                        "Schedule new downloads for: {0}, {1}",
                        scheduledDownloadTitle,
                        product.Title);

                    var scheduledDownloadEntry = new ProductDownloadEntry();
                    scheduledDownloadEntry.Type = downloadType;
                    scheduledDownloadEntry.SourceUri = actualSource;
                    scheduledDownloadEntry.Destination = destinationDirectory;

                    productDownloads.Downloads.Add(scheduledDownloadEntry);

                    await productDownloadsDataController.Update(productDownloads);

                    taskReportingController.CompleteTask();
                }
            }
        }
    }
}
