using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Reporting;
using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.Storage;

using Models.Uris;
using Models.QueryParameters;

using GOG.Controllers.PageResults;

namespace GOG.Controllers.TaskActivity
{
    public class UpdatedProductsUpdateTaskActivityController : TaskActivityController
    {
        private IRequestPageController requestPageController;
        private ISerializationController<string> serializationController;
        private IStorageController<string> storageController;

        public UpdatedProductsUpdateTaskActivityController(
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
            taskReportingController.AddTask("Update updated products from " + Uris.Paths.Account.GetFilteredProducts);

            var accountProductsPageResultsController = new AccountProductsPageResultController(
                requestPageController,
                serializationController,
                Uris.Paths.Account.GetFilteredProducts,
                QueryParameters.AccountGetUpdatedFilteredProducts,
                taskReportingController);
            var accountProductsPageResults = await accountProductsPageResultsController.GetPageResults();

            #region Extract product data

            taskReportingController.AddTask("Extract updated product data");

            var accountProductsPageResultsExtractingController = new AccountProductsPageResultsExtractingController();
            var updatedProducts = accountProductsPageResultsExtractingController.Extract(accountProductsPageResults);

            taskReportingController.CompleteTask();

            #endregion

            #region Load existing updated products

            var updatedProductsString = await storageController.Pull("updated.js");
            updatedProductsString = updatedProductsString.Replace("var updated=", "");
            var existingUpdatedProducts = serializationController.Deserialize<List<long>>(updatedProductsString);

            #endregion

            #region Save products on disk

            foreach (var updatedProduct in updatedProducts)
                if (!existingUpdatedProducts.Contains(updatedProduct.Id))
                    existingUpdatedProducts.Add(updatedProduct.Id);

            taskReportingController.AddTask("Save updated products to disk");

            updatedProductsString = "var updated=" + serializationController.Serialize(existingUpdatedProducts);
            await storageController.Push("updated.js", updatedProductsString);

            taskReportingController.CompleteTask();

            #endregion

            taskReportingController.CompleteTask();
        }
    }
}
