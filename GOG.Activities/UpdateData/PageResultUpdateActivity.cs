using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;

using Interfaces.Status;

using Models.ProductCore;

using GOG.Interfaces.Delegates.GetPageResults;
using GOG.Interfaces.NewUpdatedSelection;

using Interfaces.ActivityDefinitions;
using Interfaces.ContextDefinitions;
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
        private IIndexController<string> activityContextCreatedIndexController;

        private IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate;
        private IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate;

        private IDataController<DataType> dataController;

        private ISelectNewUpdatedAsyncDelegate<DataType> selectNewUpdatedDelegate;

        public PageResultUpdateActivity(
            AC activityContext,
            IActivityContextController activityContextController,
            IIndexController<string> activityContextCreatedIndexController,
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, DataType> itemizePageResultsDelegate,
            IDataController<DataType> dataController,
            IStatusController statusController,
            ISelectNewUpdatedAsyncDelegate<DataType> selectNewUpdatedDelegate = null) :
            base(statusController)
        {
            this.activityContext = activityContext;
            this.activityContextController = activityContextController;
            this.activityContextCreatedIndexController = activityContextCreatedIndexController;

            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.dataController = dataController;

            this.selectNewUpdatedDelegate = selectNewUpdatedDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateAllProductsTask = await statusController.CreateAsync(status, $"Update {activityContext.Item2}");

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync(updateAllProductsTask);

            var extractTask = await statusController.CreateAsync(updateAllProductsTask, $"Extract {activityContext.Item2}");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            await statusController.CompleteAsync(extractTask);

            //if (selectNewUpdatedDelegate != null)
            //{
            //    var refineDataTask = await statusController.CreateAsync(updateAllProductsTask, $"Select new/updated {activityContext.Item2}");
            //    await selectNewUpdatedDelegate.SelectNewUpdatedAsync(newProducts, refineDataTask);
            //    await statusController.CompleteAsync(refineDataTask);
            //}

            if (newProducts.Count() > 0)
            {
                var updateTask = await statusController.CreateAsync(updateAllProductsTask, $"Save {activityContext.Item2}");
                // set the date when new products were added to be able to filter those products as updated
                await activityContextCreatedIndexController.Recreate(updateAllProductsTask, activityContextController.ToString(activityContext));
                // actually update the products
                await dataController.UpdateAsync(updateTask, newProducts.ToArray());
                await statusController.CompleteAsync(updateTask);
            }

            await statusController.CompleteAsync(updateAllProductsTask);
        }
    }
}
