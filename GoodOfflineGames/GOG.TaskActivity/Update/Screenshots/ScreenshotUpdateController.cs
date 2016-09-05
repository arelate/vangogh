using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.Collection;
using Interfaces.Extraction;
using Interfaces.Network;

using Interfaces.ProductTypes;

using Models.Uris;

using GOG.Models;
using GOG.Models.Custom;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Screenshots
{
    public class ScreenshotUpdateController : TaskActivityController
    {
        private IProductTypeStorageController productStorageController;
        private ICollectionController collectionController;
        private INetworkController networkController;
        private IExtractionController screenshotExtractionController;

        public ScreenshotUpdateController(
            IProductTypeStorageController productStorageController,
            ICollectionController collectionController,
            INetworkController networkController,
            IExtractionController screenshotExtractionController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.productStorageController = productStorageController;
            this.collectionController = collectionController;
            this.networkController = networkController;
            this.screenshotExtractionController = screenshotExtractionController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Load existing products and product screenshots");
            var products = await productStorageController.Pull<Product>(ProductTypes.Product);
            var screenshots = await productStorageController.Pull<ProductScreenshots>(ProductTypes.Screenshot);
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Update all products missing screenshots");

            taskReportingController.StartTask("Get a list of updates for product screenshots");

            var productsMissingScreenshots = new List<Product>();
            foreach (var product in products)
            {
                var foundProductScreenshots = collectionController.Find(screenshots, ps => ps.Id == product.Id);
                if (foundProductScreenshots == null) productsMissingScreenshots.Add(product);
            }

            taskReportingController.CompleteTask();

            var counter = 0;

            foreach (var product in productsMissingScreenshots)
            {
                taskReportingController.StartTask(
                    string.Format(
                        "Update product screenshots {0}/{1}: {2}",
                        ++counter,
                        productsMissingScreenshots.Count,
                        product.Title));

                var foundProductScreenshots = collectionController.Find(screenshots, ps => ps.Id == product.Id);
                if (foundProductScreenshots != null) continue;

                taskReportingController.StartTask("Request product page containing screenshots information");
                var productPageUri = string.Format(Uris.Paths.GetUpdateUri(ProductTypes.Screenshot), product.Url);
                var productPageContent = await networkController.Get(productPageUri);
                taskReportingController.CompleteTask();

                taskReportingController.StartTask("Exract screenshots from the page and adding to collection");
                var extractedProductScreenshots = screenshotExtractionController.ExtractMultiple(productPageContent);

                if (extractedProductScreenshots == null) continue;

                var productScreenshots = new ProductScreenshots();
                productScreenshots.Id = product.Id;
                productScreenshots.Uris = new List<string>(extractedProductScreenshots);

                screenshots.Add(productScreenshots);
                taskReportingController.CompleteTask();

                taskReportingController.CompleteTask();
            }

            taskReportingController.StartTask("Save product screenshots to disk");
            await productStorageController.Push<ProductScreenshots>(ProductTypes.Screenshot, screenshots);
            taskReportingController.CompleteTask();

            taskReportingController.CompleteTask();
        }
    }
}
