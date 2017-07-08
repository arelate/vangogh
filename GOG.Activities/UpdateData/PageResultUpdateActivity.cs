using System.Threading.Tasks;
using System.Linq;

using Interfaces.RequestPage;
using Interfaces.Data;
using Interfaces.NewUpdatedSelection;
using Interfaces.Status;
using Interfaces.ActivityDefinitions;

using Models.ProductCore;

using GOG.Interfaces.PageResults;
using GOG.Interfaces.Extraction;

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
            var updateAllProductsTask = statusController.Create(status, $"Update {context} data");

            var productsPageResults = await pageResultsController.GetPageResults(updateAllProductsTask);

            var extractTask = statusController.Create(updateAllProductsTask, $"Extract {context} data");
            var newProducts = pageResultsExtractingController.ExtractMultiple(productsPageResults);
            statusController.Complete(extractTask);

            if (selectNewUpdatedDelegate != null)
            {
                var refineDataTask = statusController.Create(updateAllProductsTask, $"Selecting new or updated {context}");
                await selectNewUpdatedDelegate.SelectNewUpdatedAsync(newProducts, refineDataTask);
                statusController.Complete(refineDataTask);
            }

            var updateTask = statusController.Create(updateAllProductsTask, $"Update {context}");
            await dataController.UpdateAsync(updateTask, newProducts.ToArray());
            statusController.Complete(updateTask);

            statusController.Complete(updateAllProductsTask);
        }
    }
}
