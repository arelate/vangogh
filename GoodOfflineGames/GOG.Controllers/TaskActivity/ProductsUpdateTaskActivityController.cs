using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.Storage;

using Models.Uris;
using Models.QueryParameters;

using GOG.Controllers.PageResults;

namespace GOG.Controllers.TaskActivity
{
    public class ProductsUpdateTaskActivityController : TaskActivityController
    {
        private IRequestPageController requestPageController;
        private ISerializationController<string> serializationController;
        private IStorageController<string> storageController;

        public ProductsUpdateTaskActivityController(
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.requestPageController = requestPageController;
            this.serializationController = serializationController;
            this.storageController = storageController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.AddTask("Update products from " + Uris.Paths.Games.AjaxFiltered);

            var productsPageResultsController = new ProductsPageResultController(
                requestPageController,
                serializationController,
                Uris.Paths.Games.AjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                taskReportingController);
            var productsPageResults = await productsPageResultsController.GetPageResults();

            #region Extract product data

            taskReportingController.AddTask("Extract product data");

            var productsPageResultsExtractingController = new ProductsPageResultsExtractingController();

            var products = productsPageResultsExtractingController.Extract(productsPageResults);

            taskReportingController.CompleteTask();

            #endregion

            #region Save products on disk

            taskReportingController.AddTask("Save products to disk");

            var productsData = "var products=" + serializationController.Serialize(products);
            await storageController.Push("products.js", productsData);

            taskReportingController.CompleteTask();

            #endregion

            taskReportingController.CompleteTask();
        }
    }
}
