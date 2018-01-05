using System.Threading.Tasks;
using System.Linq;

using Interfaces.RequestPage;
using Interfaces.Data;
using Interfaces.Status;
using Interfaces.ContextDefinitions;

using Models.ProductCore;

using GOG.Interfaces.PageResults;
using GOG.Interfaces.Extraction;
using GOG.Interfaces.NewUpdatedSelection;

namespace GOG.Activities.UpdateData
{
    public class PageResultUpdateActivity<PageType, Type> : Activity
        where PageType : Models.PageResult
        where Type : ProductCore
    {
        private Context context;

        private IPageResultsController<PageType> pageResultsController;
        private IPageResultsExtractionController<PageType, Type> pageResultsExtractingController;

        private IRequestPageController requestPageController;
        private IDataController<Type> dataController;

        private ISelectNewUpdatedAsyncDelegate<Type> selectNewUpdatedDelegate;

        public PageResultUpdateActivity(
            Context context,
            IPageResultsController<PageType> pageResultsController,
            IPageResultsExtractionController<PageType, Type> pageResultsExtractingController,
            IRequestPageController requestPageController,
            IDataController<Type> dataController,
            IStatusController statusController,
            ISelectNewUpdatedAsyncDelegate<Type> selectNewUpdatedDelegate = null) :
            base(statusController)
        {
            this.context = context;

            this.pageResultsController = pageResultsController;
            this.pageResultsExtractingController = pageResultsExtractingController;

            this.requestPageController = requestPageController;
            this.dataController = dataController;

            this.selectNewUpdatedDelegate = selectNewUpdatedDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateAllProductsTask = await statusController.CreateAsync(status, $"Update {context}");

            var productsPageResults = await pageResultsController.GetPageResults(updateAllProductsTask);

            var extractTask = await statusController.CreateAsync(updateAllProductsTask, $"Extract {context}");
            var newProducts = pageResultsExtractingController.ExtractMultiple(productsPageResults);
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
