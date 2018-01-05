using System.Threading.Tasks;
using System.Linq;

using Interfaces.Data;
using Interfaces.Status;

using GOG.Interfaces.UpdateScreenshots;

using GOG.Models;

namespace GOG.Activities.UpdateData
{
    public class ScreenshotUpdateActivity : Activity
    {
        private IDataController<Product> productsDataController;
        private IDataController<long> screenshotsIndexDataController;
        private IUpdateScreenshotsDelegate<Product> updateProductScreenshotsDelegate;

        public ScreenshotUpdateActivity(
            IDataController<Product> productsDataController,
            IDataController<long> screenshotsIndexDataController,
            IUpdateScreenshotsDelegate<Product> updateProductScreenshotsDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.productsDataController = productsDataController;
            this.screenshotsIndexDataController = screenshotsIndexDataController;
            this.updateProductScreenshotsDelegate = updateProductScreenshotsDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateProductsScreenshotsTask = await statusController.CreateAsync(status, "Update Screenshots");

            var getUpdatesListTask = await statusController.CreateAsync(updateProductsScreenshotsTask, "Get updates");
            var productsMissingScreenshots = (await productsDataController.EnumerateIdsAsync(getUpdatesListTask)).Except(
                await screenshotsIndexDataController.EnumerateIdsAsync(getUpdatesListTask));
            await statusController.CompleteAsync(getUpdatesListTask);

            var counter = 0;

            foreach (var id in productsMissingScreenshots)
            {
                var product = await productsDataController.GetByIdAsync(id, updateProductsScreenshotsTask);

                if (product == null)
                {
                    await statusController.InformAsync(
                        updateProductsScreenshotsTask,
                        $"Product {id} was not found as product or accountProduct, but marked as missing screenshots");
                    continue;
                }

                await statusController.UpdateProgressAsync(
                    updateProductsScreenshotsTask,
                    ++counter,
                    productsMissingScreenshots.Count(),
                    product.Title);

                await updateProductScreenshotsDelegate.UpdateProductScreenshots(product, updateProductsScreenshotsTask);

            }

            await statusController.CompleteAsync(updateProductsScreenshotsTask);
        }
    }
}
