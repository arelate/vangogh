using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;
using Delegates.Activities;
using Delegates.Data.Models.ProductTypes;
using GOG.Delegates.Data.Models.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Server;
using Models.ProductTypes;

namespace GOG.Delegates.Server.Update
{
    [RespondsToRequests(Method = "update", Collection = "screenshots")]
    public class UpdateScreenshotsAsyncDelegate : IProcessAsyncDelegate
    {
        private readonly IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate;
        private readonly IGetDataAsyncDelegate<ProductScreenshots, Product> getScreenshotsByProductAsyncDelegate;
        private readonly IUpdateAsyncDelegate<ProductScreenshots> updateProductScreenshotsAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GetProductByIdAsyncDelegate),
            typeof(GetProductScreenshotsByProductAsyncDelegate),
            typeof(UpdateProductScreenshotsAsyncDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public UpdateScreenshotsAsyncDelegate(
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate,
            IGetDataAsyncDelegate<ProductScreenshots, Product> getScreenshotsByProductAsyncDelegate,
            IUpdateAsyncDelegate<ProductScreenshots> updateProductScreenshotsAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getProductByIdAsyncDelegate = getProductByIdAsyncDelegate;
            this.getScreenshotsByProductAsyncDelegate = getScreenshotsByProductAsyncDelegate;
            this.updateProductScreenshotsAsyncDelegate = updateProductScreenshotsAsyncDelegate;
            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task ProcessAsync(IDictionary<string, IEnumerable<string>> parameters)
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

                // 
                var productScreenshots = await getScreenshotsByProductAsyncDelegate.GetDataAsync(product);
                
                startDelegate.Start("Add product screenshots");
                await updateProductScreenshotsAsyncDelegate.UpdateAsync(productScreenshots);
                completeDelegate.Complete();
            }

            completeDelegate.Complete();

            completeDelegate.Complete();
        }
    }
}