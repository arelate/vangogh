using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.UpdateScreenshots;
using Attributes;
using GOG.Models;
using Models.ProductTypes;
using Delegates.Activities;

namespace GOG.Delegates.Respond.Update.ProductTypes
{
    [RespondsToRequests(Method = "update", Collection = "screenshots")]
    public class RespondToUpdateScreenshotsRequestDelegate : IRespondAsyncDelegate
    {
        private readonly IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate;
        private readonly IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate),
            typeof(GOG.Delegates.UpdateScreenshots.UpdateScreenshotsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public RespondToUpdateScreenshotsRequestDelegate(
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IUpdateScreenshotsAsyncDelegate<Product> updateScreenshotsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getProductByIdAsyncDelegate = getProductByIdAsyncDelegate;
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
                var product = await getProductByIdAsyncDelegate.GetDataAsync(id);

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