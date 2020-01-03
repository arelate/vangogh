using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Delegates.Respond;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using GOG.Interfaces.Delegates.UpdateScreenshots;

using Attributes;

using GOG.Models;

using Models.ProductTypes;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method="update", Collection="screenshots")]
    public class RespondToUpdateScreenshotsRequestDelegate : IRespondAsyncDelegate
    {
        readonly IDataController<Product> productsDataController;
        readonly IDataController<ProductScreenshots> productScreenshotsDataController;
        readonly IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate;
        readonly IActionLogController actionLogController;

        [Dependencies(
            DependencyContext.Default,
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Data.ProductTypes.ProductScreenshotsDataController,Controllers",
            "GOG.Delegates.UpdateScreenshots.UpdateScreenshotsAsyncDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToUpdateScreenshotsRequestDelegate(
            IDataController<Product> productsDataController,
            IDataController<ProductScreenshots> productScreenshotsDataController,
            IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate,
            IActionLogController actionLogController)
        {
            this.productsDataController = productsDataController;
            this.productScreenshotsDataController = productScreenshotsDataController;
            this.updateScreenshotsAsyncDelegate = updateScreenshotsAsyncDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            actionLogController.StartAction("Update Screenshots");

            actionLogController.StartAction("Get updates");
            var productsMissingScreenshots = new List<long>();
            // TODO: Properly enumerate productsMissingScreenshots
            // (productsDataController.ItemizeAllAsync(getUpdatesListTask)).Except(
            //     productScreenshotsDataController.ItemizeAllAsync(getUpdatesListTask));
            actionLogController.CompleteAction();

            actionLogController.StartAction("Update missing screenshots");
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

                actionLogController.IncrementActionProgress();

                await updateScreenshotsAsyncDelegate.UpdateScreenshotsAsync(product);
            }
            actionLogController.CompleteAction();

            actionLogController.CompleteAction();
        }
    }
}
