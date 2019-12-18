using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Interfaces.Activity;

using GOG.Interfaces.Delegates.UpdateScreenshots;

using Attributes;

using GOG.Models;

using Models.ProductTypes;

namespace GOG.Activities.Update
{
    public class UpdateScreenshotsActivity : IActivity
    {
        readonly IDataController<Product> productsDataController;
        readonly IDataController<ProductScreenshots> productScreenshotsDataController;
        readonly IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate;
        readonly IResponseLogController responseLogController;

        [Dependencies(
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Data.ProductTypes.ProductScreenshotsDataController,Controllers",
            "GOG.Delegates.UpdateScreenshots.UpdateScreenshotsAsyncDelegate,GOG.Delegates",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public UpdateScreenshotsActivity(
            IDataController<Product> productsDataController,
            IDataController<ProductScreenshots> productScreenshotsDataController,
            IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate,
            IResponseLogController responseLogController)
        {
            this.productsDataController = productsDataController;
            this.productScreenshotsDataController = productScreenshotsDataController;
            this.updateScreenshotsAsyncDelegate = updateScreenshotsAsyncDelegate;
            this.responseLogController = responseLogController;
        }

        public async Task ProcessActivityAsync()
        {
            responseLogController.OpenResponseLog("Update Screenshots");

            responseLogController.StartAction("Get updates");
            var productsMissingScreenshots = new List<long>();
            // TODO: Properly enumerate productsMissingScreenshots
            // (productsDataController.ItemizeAllAsync(getUpdatesListTask)).Except(
            //     productScreenshotsDataController.ItemizeAllAsync(getUpdatesListTask));
            responseLogController.CompleteAction();

            responseLogController.StartAction("Update missing screenshots");
            foreach (var id in productsMissingScreenshots)
            {
                var product = await productsDataController.GetByIdAsync(id);

                if (product == null)
                {
                    // await statusController.InformAsync(
                    //     updateProductsScreenshotsTask,
                    //     $"Product {id} was not found as product or accountProduct, but marked as missing screenshots");
                    continue;
                }

                responseLogController.IncrementActionProgress();

                await updateScreenshotsAsyncDelegate.UpdateScreenshotsAsync(product);
            }
            responseLogController.CompleteAction();

            responseLogController.CloseResponseLog();
        }
    }
}
