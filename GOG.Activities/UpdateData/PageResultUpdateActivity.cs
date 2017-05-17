using System.Threading.Tasks;
using System.Linq;

using Interfaces.RequestPage;
using Interfaces.Data;
using Interfaces.NewUpdatedSelection;
using Interfaces.Status;

using Models.ProductCore;

using GOG.Interfaces.PageResults;
using GOG.Interfaces.Extraction;

namespace GOG.Activities.UpdateData
{
    public class PageResultUpdateActivity<PageType, Type> : Activity
        where PageType : Models.PageResult
        where Type : ProductCore
    {
        private string productParameter;

        private IPageResultsController<PageType> pageResultsController;
        private IPageResultsExtractionController<PageType, Type> pageResultsExtractingController;

        private IRequestPageController requestPageController;
        private IDataController<Type> dataController;

        private ISelectNewUpdatedDelegate<Type> selectNewUpdatedDelegate;

        public PageResultUpdateActivity(
            string productParameter,
            IPageResultsController<PageType> pageResultsController,
            IPageResultsExtractionController<PageType, Type> pageResultsExtractingController,
            IRequestPageController requestPageController,
            IDataController<Type> dataController,
            IStatusController statusController,
            ISelectNewUpdatedDelegate<Type> selectNewUpdatedDelegate = null) :
            base(statusController)
        {
            this.productParameter = productParameter;

            this.pageResultsController = pageResultsController;
            this.pageResultsExtractingController = pageResultsExtractingController;

            this.requestPageController = requestPageController;
            this.dataController = dataController;

            this.selectNewUpdatedDelegate = selectNewUpdatedDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var updateAllProductsTask = statusController.Create(status, $"Update {productParameter} data");

            var productsPageResults = await pageResultsController.GetPageResults(updateAllProductsTask);

            var extractTask = statusController.Create(updateAllProductsTask, $"Extract {productParameter} data");
            var newProducts = pageResultsExtractingController.ExtractMultiple(productsPageResults);
            statusController.Complete(extractTask);

            if (selectNewUpdatedDelegate != null)
            {
                var refineDataTask = statusController.Create(updateAllProductsTask, $"Selecting new or updated {productParameter}");
                await selectNewUpdatedDelegate.SelectNewUpdatedAsync(newProducts, refineDataTask);
                statusController.Complete(refineDataTask);
            }

            var updateTask = statusController.Create(updateAllProductsTask, $"Update {productParameter}");
            await dataController.UpdateAsync(updateTask, newProducts.ToArray());
            statusController.Complete(updateTask);

            statusController.Complete(updateAllProductsTask);
        }
    }
}
