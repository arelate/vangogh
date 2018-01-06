using System.Threading.Tasks;
using System.Linq;

using Interfaces.Controllers.Data;
using Interfaces.Status;

using GOG.Interfaces.Delegates.UpdateScreenshots;

using GOG.Models;

namespace GOG.Activities.UpdateData
{
    public class UpdateScreenshotsActivity : Activity
    {
        private IDataController<Product> productsDataController;
        private IDataController<long> screenshotsIndexDataController;
        private IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate;

        public UpdateScreenshotsActivity(
            IDataController<Product> productsDataController,
            IDataController<long> screenshotsIndexDataController,
            IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.productsDataController = productsDataController;
            this.screenshotsIndexDataController = screenshotsIndexDataController;
            this.updateScreenshotsAsyncDelegate = updateScreenshotsAsyncDelegate;
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

                await updateScreenshotsAsyncDelegate.UpdateScreenshotsAsync(product, updateProductsScreenshotsTask);

            }

            await statusController.CompleteAsync(updateProductsScreenshotsTask);
        }
    }
}
