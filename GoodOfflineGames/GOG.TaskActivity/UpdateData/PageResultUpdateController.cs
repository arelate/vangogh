using System.Threading.Tasks;
using System.Linq;

using Interfaces.RequestPage;
using Interfaces.Data;
using Interfaces.Collection;
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

        private ICollectionProcessingController<Type> collectionProcessingController;

        public PageResultUpdateController(
            string productParameter,
            IPageResultsController<PageType> pageResultsController,
            IPageResultsExtractionController<PageType, Type> pageResultsExtractingController,
            IRequestPageController requestPageController,
            IDataController<Type> dataController,
            ITaskStatusController taskStatusController,
            ICollectionProcessingController<Type> collectionProcessingController = null) :
            base(taskStatusController)
        {
            this.productParameter = productParameter;

            this.pageResultsController = pageResultsController;
            this.pageResultsExtractingController = pageResultsExtractingController;

            this.requestPageController = requestPageController;
            this.dataController = dataController;

            this.collectionProcessingController = collectionProcessingController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var updateAllProductsTask = taskStatusController.Create(taskStatus, "Update products information");

            var productsPageResults = await pageResultsController.GetPageResults(updateAllProductsTask);

            var extractTask = taskStatusController.Create(updateAllProductsTask, "Extract product data");
            var newProducts = pageResultsExtractingController.ExtractMultiple(productsPageResults);
            taskStatusController.Complete(extractTask);

            var updateTask = taskStatusController.Create(updateAllProductsTask, "Update products");
            await dataController.UpdateAsync(updateTask, newProducts.ToArray());
            taskStatusController.Complete(updateTask);

            var processingTask = taskStatusController.Create(updateAllProductsTask, "Post-processing products");

            if (collectionProcessingController != null)
                await collectionProcessingController.Process(newProducts, processingTask);

            taskStatusController.Complete(processingTask);

            taskStatusController.Complete(updateAllProductsTask);
        }
    }
}
