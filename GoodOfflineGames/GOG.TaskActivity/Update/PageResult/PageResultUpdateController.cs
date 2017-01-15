using System.Threading.Tasks;
using System.Linq;

using Interfaces.RequestPage;
using Interfaces.ProductTypes;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.Uris;
using Models.ProductCore;

using GOG.Interfaces.PageResults;
using GOG.Interfaces.Extraction;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.PageResult
{
    public class PageResultUpdateController<PageType, Type> : TaskActivityController
        where PageType : Models.PageResult
        where Type : ProductCore
    {
        private ProductTypes productType;

        private IPageResultsController<PageType> pageResultsController;
        private IPageResultsExtractionController<PageType, Type> pageResultsExtractingController;

        private IRequestPageController requestPageController;
        private IDataController<Type> dataController;

        public PageResultUpdateController(
            ProductTypes productType,
            IPageResultsController<PageType> pageResultsController,
            IPageResultsExtractionController<PageType, Type> pageResultsExtractingController,
            IRequestPageController requestPageController,
            IDataController<Type> dataController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                taskStatus,
                taskStatusController)
        {
            this.productType = productType;

            this.pageResultsController = pageResultsController;
            this.pageResultsExtractingController = pageResultsExtractingController;

            this.requestPageController = requestPageController;
            this.dataController = dataController;
        }

        public override async Task ProcessTaskAsync()
        {
            var updateAllProductsTask = taskStatusController.Create(taskStatus, "Update products information");

            var productsPageResults = await pageResultsController.GetPageResults(updateAllProductsTask);

            var extractTask = taskStatusController.Create(updateAllProductsTask, "Extract product data");
            var products = pageResultsExtractingController.Extract(productsPageResults);
            taskStatusController.Complete(extractTask);

            var updateTask = taskStatusController.Create(updateAllProductsTask, "Update existing products");
            await dataController.UpdateAsync(products.ToArray());
            taskStatusController.Complete(updateTask);

            taskStatusController.Complete(updateAllProductsTask);
        }
    }
}
