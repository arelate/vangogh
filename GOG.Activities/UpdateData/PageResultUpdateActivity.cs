using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;

using Interfaces.Status;

using Models.ProductCore;

using GOG.Interfaces.Delegates.GetPageResults;

using Interfaces.ActivityContext;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace GOG.Activities.UpdateData
{
    public class PageResultUpdateActivity<PageType, DataType> : Activity
        where PageType : Models.PageResult
        where DataType : ProductCore
    {
        private AC activityContext;
        private IActivityContextController activityContextController;

        private IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate;
        private IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate;

        private IDataController<DataType> dataController;

        public PageResultUpdateActivity(
            AC activityContext,
            IActivityContextController activityContextController,
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate,
            IDataController<DataType> dataController,
            IStatusController statusController) :
            base(statusController)
        {
            this.activityContext = activityContext;
            this.activityContextController = activityContextController;

            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.dataController = dataController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateAllProductsTask = await statusController.CreateAsync(status, $"Update {activityContext.Item2}");

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync(updateAllProductsTask);

            var extractTask = await statusController.CreateAsync(updateAllProductsTask, $"Extract {activityContext.Item2}");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            await statusController.CompleteAsync(extractTask);

            if (newProducts.Count() > 0)
            {
                var updateTask = await statusController.CreateAsync(updateAllProductsTask, $"Save {activityContext.Item2}");
                var current = 0;

                foreach (var product in newProducts)
                {
                    await statusController.UpdateProgressAsync(updateTask, ++current, newProducts.Count(), product.Title);

                    await dataController.UpdateAsync(product, updateTask);
                }
                
                await statusController.CompleteAsync(updateTask);
            }

            await statusController.CompleteAsync(updateAllProductsTask);
        }
    }
}
