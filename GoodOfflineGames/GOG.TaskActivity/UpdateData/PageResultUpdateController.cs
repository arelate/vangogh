using System.Threading.Tasks;
using System.Linq;

using Interfaces.RequestPage;
using Interfaces.Data;
using Interfaces.DataRefinement;
using Interfaces.TaskStatus;

using Models.ProductCore;

using GOG.Interfaces.PageResults;
using GOG.Interfaces.Extraction;

namespace GOG.TaskActivities.UpdateData
{
    public class PageResultUpdateController<PageType, Type> : TaskActivityController
        where PageType : Models.PageResult
        where Type : ProductCore
    {
        private string productParameter;

        private IPageResultsController<PageType> pageResultsController;
        private IPageResultsExtractionController<PageType, Type> pageResultsExtractingController;

        private IRequestPageController requestPageController;
        private IDataController<Type> dataController;

        private IDataRefinementController<Type> dataRefinementController;

        public PageResultUpdateController(
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

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var updateAllProductsTask = taskStatusController.Create(taskStatus, $"Update {productParameter} data");

            var productsPageResults = await pageResultsController.GetPageResults(updateAllProductsTask);

            var extractTask = taskStatusController.Create(updateAllProductsTask, $"Extract {productParameter} data");
            var newProducts = pageResultsExtractingController.ExtractMultiple(productsPageResults);
            taskStatusController.Complete(extractTask);

            var updateTask = taskStatusController.Create(updateAllProductsTask, $"Update {productParameter}");
            await dataController.UpdateAsync(updateTask, newProducts.ToArray());
            taskStatusController.Complete(updateTask);

            var processingTask = taskStatusController.Create(updateAllProductsTask, $"Post-processing {productParameter}");

            await dataRefinementController?.RefineData(newProducts, processingTask);

            taskStatusController.Complete(processingTask);

            taskStatusController.Complete(updateAllProductsTask);
        }
    }
}
