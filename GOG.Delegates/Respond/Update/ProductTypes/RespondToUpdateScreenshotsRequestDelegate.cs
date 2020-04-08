using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Interfaces.Delegates.Respond;
using Interfaces.Controllers.Data;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.UpdateScreenshots;
using Attributes;
using GOG.Models;
using Models.ProductTypes;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method = "update", Collection = "screenshots")]
    public class RespondToUpdateScreenshotsRequestDelegate : IRespondAsyncDelegate
    {
        private readonly IDataController<Product> productsDataController;
        private readonly IDataController<ProductScreenshots> productScreenshotsDataController;
        private readonly IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "Controllers.Data.ProductTypes.ProductScreenshotsDataController,Controllers",
            "GOG.Delegates.UpdateScreenshots.UpdateScreenshotsAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToUpdateScreenshotsRequestDelegate(
            IDataController<Product> productsDataController,
            IDataController<ProductScreenshots> productScreenshotsDataController,
            IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.productsDataController = productsDataController;
            this.productScreenshotsDataController = productScreenshotsDataController;
            this.updateScreenshotsAsyncDelegate = updateScreenshotsAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            startDelegate.Start("Update Screenshots");

            startDelegate.Start("Get updates");
            var productsMissingScreenshots = new List<long>();
            // TODO: Properly enumerate productsMissingScreenshots
            // (productsDataController.ItemizeAllAsync(getUpdatesListTask)).Except(
            //     productScreenshotsDataController.ItemizeAllAsync(getUpdatesListTask));
            completeDelegate.Complete();

            startDelegate.Start("Update missing screenshots");
            foreach (var id in productsMissingScreenshots)
            {
                var product = await productsDataController.GetByIdAsync(id);

                if (product == null)
                    // await statusController.InformAsync(
                    //     updateProductsScreenshotsTask,
                    //     $"Product {id} was not found as product or accountProduct, but marked as missing screenshots");
                    continue;

                setProgressDelegate.SetProgress();

                await updateScreenshotsAsyncDelegate.UpdateScreenshotsAsync(product);
            }

            completeDelegate.Complete();

            completeDelegate.Complete();
        }
    }
}