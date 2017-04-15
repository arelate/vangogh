using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Extraction;
using Interfaces.Network;
using Interfaces.Data;
using Interfaces.Status;
using Interfaces.UpdateUri;

using Models.ProductScreenshots;
using Models.Units;
using Models.FlightPlan;

using GOG.Models;

using Models.ProductCore;

namespace GOG.Activities.UpdateData
{
    public class ScreenshotUpdateActivity : Activity
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
            IStatusController statusController) :
            base(statusController)
        {
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.screenshotsDataController = screenshotsDataController;
            this.productsDataController = productsDataController;
            this.networkController = networkController;
            this.screenshotExtractionController = screenshotExtractionController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateAllTask = statusController.Create(status, "Update all products missing screenshots");

            var getUpdatesListTask = statusController.Create(updateAllTask, "Get a list of updates for product screenshots");

            var productsMissingScreenshots = productsDataController.EnumerateIds().Except(
                screenshotsDataController.EnumerateIds());

            statusController.Complete(getUpdatesListTask);

            var counter = 0;

            var updateProductsScreenshotsTask = statusController.Create(updateAllTask, "Update products screenshots");

            foreach (var id in productsMissingScreenshots)
            {
                var product = await productsDataController.GetByIdAsync(id);

                if (product == null)
                {
                    statusController.Inform(
                        updateProductsScreenshotsTask,
                        $"Product {id} was not found as product or accountProduct, but marked as missing screenshots");
                    continue;
                }

                statusController.UpdateProgress(
                    updateProductsScreenshotsTask,
                    ++counter,
                    productsMissingScreenshots.Count(),
                    product.Title,
                    ProductUnits.Products);

                var requestProductPageTask = statusController.Create(updateProductsScreenshotsTask, "Request product page containing screenshots information");
                var productPageUri = string.Format(getUpdateUriDelegate.GetUpdateUri(Parameters.Screenshots), product.Url);
                var productPageContent = await networkController.Get(requestProductPageTask, productPageUri);
                statusController.Complete(requestProductPageTask);

                var extractScreenshotsTask = statusController.Create(updateProductsScreenshotsTask, "Exract screenshots from the page");
                var extractedProductScreenshots = screenshotExtractionController.ExtractMultiple(productPageContent);

                if (extractedProductScreenshots == null) continue;

                var productScreenshots = new ProductScreenshots()
                {
                    Id = product.Id,
                    Title = product.Title,
                    Uris = new List<string>(extractedProductScreenshots)
                };
                statusController.Complete(extractScreenshotsTask);

                var updateProductScreenshotsTask = statusController.Create(updateProductsScreenshotsTask, "Add product screenshots");
                await screenshotsDataController.UpdateAsync(updateProductScreenshotsTask, productScreenshots);
                statusController.Complete(updateProductScreenshotsTask);
            }

            statusController.Complete(updateProductsScreenshotsTask);

            statusController.Complete(updateAllTask);
        }
    }
}
