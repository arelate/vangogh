using System.Threading.Tasks;
using System.Linq;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;
using Interfaces.Status;

using GOG.Interfaces.Delegates.UpdateScreenshots;

using GOG.Models;

using Models.ProductScreenshots;

namespace GOG.Activities.UpdateData
{
    public class UpdateScreenshotsActivity : Activity
    {
        readonly IDataController<Product> productsDataController;
        readonly IDataController<ProductScreenshots> productScreenshotsDataController;
        readonly IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate;

        public UpdateScreenshotsActivity(
            IDataController<Product> productsDataController,
            IDataController<ProductScreenshots> productScreenshotsDataController,
            IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.productsDataController = productsDataController;
            this.productScreenshotsDataController = productScreenshotsDataController;
            this.updateScreenshotsAsyncDelegate = updateScreenshotsAsyncDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateProductsScreenshotsTask = await statusController.CreateAsync(status, "Update Screenshots");

            var getUpdatesListTask = await statusController.CreateAsync(updateProductsScreenshotsTask, "Get updates");
            var productsMissingScreenshots = (await productsDataController.ItemizeAllAsync(getUpdatesListTask)).Except(
                await productScreenshotsDataController.ItemizeAllAsync(getUpdatesListTask));
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
