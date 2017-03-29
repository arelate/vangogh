using System.Threading.Tasks;
using System.Linq;

using Interfaces.RequestPage;
using Interfaces.Data;
using Interfaces.DataRefinement;
using Interfaces.TaskStatus;

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

        private IDataRefinementController<Type> dataRefinementController;

        public PageResultUpdateActivity(
            string productParameter,
            IPageResultsController<PageType> pageResultsController,
            IPageResultsExtractionController<PageType, Type> pageResultsExtractingController,
            IRequestPageController requestPageController,
            IDataController<Type> dataController,
            ITaskStatusController taskStatusController,
            IDataRefinementController<Type> dataRefinementController = null) :
            base(taskStatusController)
        {
            this.productParameter = productParameter;

            this.pageResultsController = pageResultsController;
            this.pageResultsExtractingController = pageResultsExtractingController;

            this.requestPageController = requestPageController;
            this.dataController = dataController;

            this.dataRefinementController = dataRefinementController;
        }

        public override async Task ProcessActivityAsync(ITaskStatus taskStatus)
        {
            var updateAllProductsTask = taskStatusController.Create(taskStatus, $"Update {productParameter} data");

            var productsPageResults = await pageResultsController.GetPageResults(updateAllProductsTask);

            var extractTask = taskStatusController.Create(updateAllProductsTask, $"Extract {productParameter} data");
            var newProducts = pageResultsExtractingController.ExtractMultiple(productsPageResults);
            taskStatusController.Complete(extractTask);

            var refineDataTask = taskStatusController.Create(updateAllProductsTask, $"Refining {productParameter}");
            if (dataRefinementController != null)
                await dataRefinementController.RefineData(newProducts, refineDataTask);
            taskStatusController.Complete(refineDataTask);

            var updateTask = taskStatusController.Create(updateAllProductsTask, $"Update {productParameter}");
            await dataController.UpdateAsync(updateTask, newProducts.ToArray());
            taskStatusController.Complete(updateTask);

            taskStatusController.Complete(updateAllProductsTask);
        }
    }
}
