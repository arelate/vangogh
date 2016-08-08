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
    public class AccountProductsUpdateTaskActivityController : TaskActivityController
    {
        private IRequestPageController requestPageController;
        private ISerializationController<string> serializationController;
        private IStorageController<string> storageController;

        public AccountProductsUpdateTaskActivityController(
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
            taskReportingController.AddTask("Update account products from " + Uris.Paths.Account.GetFilteredProducts);

            var accountProductsPageResultsController = new AccountProductsPageResultController(
                requestPageController,
                serializationController,
                Uris.Paths.Account.GetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                taskReportingController);
            var accountProductsPageResults = await accountProductsPageResultsController.GetPageResults();

            #region Extract product data

            taskReportingController.AddTask("Extract account product data");

            var accountProductsPageResultsExtractingController = new AccountProductsPageResultsExtractingController();
            var accountProducts = accountProductsPageResultsExtractingController.Extract(accountProductsPageResults);

            taskReportingController.CompleteTask();

            #endregion

            #region Save products on disk

            taskReportingController.AddTask("Save account products to disk");

            var accountProductsString = "var accountProducts=" + serializationController.Serialize(accountProducts);
            await storageController.Push("accountProducts.js", accountProductsString);

            taskReportingController.CompleteTask();

            #endregion

            taskReportingController.CompleteTask();
        }
    }
}
