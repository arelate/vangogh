using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Extraction;
using Interfaces.Network;
using Interfaces.Data;
using Interfaces.ProductTypes;
using Interfaces.TaskStatus;

using Models.Uris;

using GOG.Models;
using Models.ProductScreenshots;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Screenshots
{
    public class ScreenshotUpdateController : TaskActivityController
    {
        private IDataController<ProductScreenshots> screenshotsDataController;
        private IDataController<long> scheduledScreenshotsUpdatesDataController;
        private IDataController<Product> productsDataController;
        private INetworkController networkController;
        private IExtractionController screenshotExtractionController;

        public ScreenshotUpdateController(
            IDataController<ProductScreenshots> screenshotsDataController,
            IDataController<long> scheduledScreenshotsUpdatesDataController,
            IDataController<Product> productsDataController,
            INetworkController networkController,
            IExtractionController screenshotExtractionController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                taskStatus,
                taskStatusController)
        {
            this.screenshotsDataController = screenshotsDataController;
            this.scheduledScreenshotsUpdatesDataController = scheduledScreenshotsUpdatesDataController;
            this.productsDataController = productsDataController;
            this.networkController = networkController;
            this.screenshotExtractionController = screenshotExtractionController;
        }

        public override async Task ProcessTaskAsync()
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
                    "product(s)");

                var requestProductPageTask = taskStatusController.Create(updateProductsScreenshotsTask, "Request product page containing screenshots information");
                var productPageUri = string.Format(Uris.Paths.GetUpdateUri(ProductTypes.Screenshot), product.Url);
                var productPageContent = await networkController.Get(productPageUri);
                taskStatusController.Complete(requestProductPageTask);

                var extractScreenshotsTask = taskStatusController.Create(updateProductsScreenshotsTask, "Exract screenshots from the page");
                var extractedProductScreenshots = screenshotExtractionController.ExtractMultiple(productPageContent);

                if (extractedProductScreenshots == null) continue;

                var productScreenshots = new ProductScreenshots()
                {
                    Id = product.Id,
                    Uris = new List<string>(extractedProductScreenshots)
                };
                taskStatusController.Complete(extractScreenshotsTask);

                var updateProductScreenshotsTask = taskStatusController.Create(updateProductsScreenshotsTask, "Update product screenshots");
                await screenshotsDataController.UpdateAsync(productScreenshots);
                taskStatusController.Complete(updateProductScreenshotsTask);

                var scheduleScreenshotUpdateTask = taskStatusController.Create(updateProductsScreenshotsTask, "Schedule screenshot files update");
                await scheduledScreenshotsUpdatesDataController.UpdateAsync(product.Id);
                taskStatusController.Complete(scheduleScreenshotUpdateTask);
            }

            taskStatusController.Complete(updateProductsScreenshotsTask);

            taskStatusController.Complete(updateAllTask);
        }
    }
}
