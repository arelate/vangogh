using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Data;
using Interfaces.Status;
using Interfaces.ContextDefinitions;

using Models.ProductCore;

using GOG.Interfaces.Delegates.GetPageResults;
using GOG.Interfaces.NewUpdatedSelection;

namespace GOG.Activities.UpdateData
{
    public class PageResultUpdateActivity<PageType, Type> : Activity
        where PageType : Models.PageResult
        where Type : ProductCore
    {
        private Context context;

        private IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate;
        private IItemizeDelegate<IList<PageType>, Type> itemizePageResultsDelegate;

        private IDataController<Type> dataController;

        private ISelectNewUpdatedAsyncDelegate<Type> selectNewUpdatedDelegate;

        public PageResultUpdateActivity(
            Context context,
            IGetPageResultsAsyncDelegate<PageType> getPageResultsAsyncDelegate,
            IItemizeDelegate<IList<PageType>, Type> itemizePageResultsDelegate,
            IDataController<Type> dataController,
            IStatusController statusController,
            ISelectNewUpdatedAsyncDelegate<Type> selectNewUpdatedDelegate = null) :
            base(statusController)
        {
            this.context = context;

            this.getPageResultsAsyncDelegate = getPageResultsAsyncDelegate;
            this.itemizePageResultsDelegate = itemizePageResultsDelegate;

            this.dataController = dataController;

            this.selectNewUpdatedDelegate = selectNewUpdatedDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateAllProductsTask = await statusController.CreateAsync(status, $"Update {context}");

            var productsPageResults = await getPageResultsAsyncDelegate.GetPageResultsAsync(updateAllProductsTask);

            var extractTask = await statusController.CreateAsync(updateAllProductsTask, $"Extract {context}");
            var newProducts = itemizePageResultsDelegate.Itemize(productsPageResults);
            await statusController.CompleteAsync(extractTask);

            if (selectNewUpdatedDelegate != null)
            {
                var refineDataTask = await statusController.CreateAsync(updateAllProductsTask, $"Select new/updated {context}");
                await selectNewUpdatedDelegate.SelectNewUpdatedAsync(newProducts, refineDataTask);
                await statusController.CompleteAsync(refineDataTask);
            }

            var updateTask = await statusController.CreateAsync(updateAllProductsTask, $"Save {context}");
            await dataController.UpdateAsync(updateTask, newProducts.ToArray());
            await statusController.CompleteAsync(updateTask);

            await statusController.CompleteAsync(updateAllProductsTask);
        }
    }
}
