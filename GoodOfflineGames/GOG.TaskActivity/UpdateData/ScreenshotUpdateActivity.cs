using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Extraction;
using Interfaces.Network;
using Interfaces.Data;
using Interfaces.TaskStatus;
using Interfaces.UpdateUri;

using Models.ProductScreenshots;
using Models.Units;
using Models.ActivityParameters;

using GOG.Models;

namespace GOG.TaskActivities.UpdateData
{
    public class ScreenshotUpdateActivity : TaskActivityController
    {
        private IGetUpdateUriDelegate<string> getUpdateUriDelegate;
        private IDataController<ProductScreenshots> screenshotsDataController;
        private IDataController<Product> productsDataController;
        private INetworkController networkController;
        private IStringExtractionController screenshotExtractionController;

        public ScreenshotUpdateActivity(
            IGetUpdateUriDelegate<string> getUpdateUriDelegate,
            IDataController<ProductScreenshots> screenshotsDataController,
            IDataController<Product> productsDataController,
            INetworkController networkController,
            IStringExtractionController screenshotExtractionController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.screenshotsDataController = screenshotsDataController;
            this.productsDataController = productsDataController;
            this.networkController = networkController;
            this.screenshotExtractionController = screenshotExtractionController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var updateAllTask = taskStatusController.Create(taskStatus, "Update all products missing screenshots");

            var getUpdatesListTask = taskStatusController.Create(updateAllTask, "Get a list of updates for product screenshots");

            var productsMissingScreenshots = new List<long>();

            foreach (var id in productsDataController.EnumerateIds())
                if (!screenshotsDataController.ContainsId(id))
                    productsMissingScreenshots.Add(id);

            taskStatusController.Complete(getUpdatesListTask);

            var counter = 0;

            var updateProductsScreenshotsTask = taskStatusController.Create(updateAllTask, "Update products screenshots");

            foreach (var id in productsMissingScreenshots)
            {
                var product = await productsDataController.GetByIdAsync(id);

                taskStatusController.UpdateProgress(
                    updateProductsScreenshotsTask, 
                    ++counter, 
                    productsMissingScreenshots.Count,
                    product.Title,
                    ProductUnits.Products);

                var requestProductPageTask = taskStatusController.Create(updateProductsScreenshotsTask, "Request product page containing screenshots information");
                var productPageUri = string.Format(getUpdateUriDelegate.GetUpdateUri(Parameters.Screenshots), product.Url);
                var productPageContent = await networkController.Get(requestProductPageTask, productPageUri);
                taskStatusController.Complete(requestProductPageTask);

                var extractScreenshotsTask = taskStatusController.Create(updateProductsScreenshotsTask, "Exract screenshots from the page");
                var extractedProductScreenshots = screenshotExtractionController.ExtractMultiple(productPageContent);

                if (extractedProductScreenshots == null) continue;

                var productScreenshots = new ProductScreenshots()
                {
                    Id = product.Id,
                    Title = product.Title,
                    Uris = new List<string>(extractedProductScreenshots)
                };
                taskStatusController.Complete(extractScreenshotsTask);

                var updateProductScreenshotsTask = taskStatusController.Create(updateProductsScreenshotsTask, "Add product screenshots");
                await screenshotsDataController.UpdateAsync(updateProductScreenshotsTask, productScreenshots);
                taskStatusController.Complete(updateProductScreenshotsTask);
            }

            taskStatusController.Complete(updateProductsScreenshotsTask);

            taskStatusController.Complete(updateAllTask);
        }
    }
}
