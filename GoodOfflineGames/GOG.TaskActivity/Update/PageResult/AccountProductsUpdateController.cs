using Interfaces.Reporting;
using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.ProductTypes;
using Interfaces.Storage;

using Models.Uris;
using Models.QueryParameters;

using GOG.Models;

using GOG.Controllers.PageResults;
using GOG.Controllers.Extraction;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.PageResult
{
    public class AccountProductsUpdateController : PageResultUpdateController<AccountProductsPageResult, AccountProduct>
    {
        public AccountProductsUpdateController(
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            IProductTypeStorageController productStorageController,
            ITaskReportingController taskReportingController) :
            base(requestPageController,
                serializationController,
                productStorageController,
                taskReportingController)
        {
            productType = ProductTypes.AccountProduct;
            pageResultsController = new AccountProductsPageResultController(
                requestPageController,
                serializationController,
                Uris.Paths.GetUpdateUri(productType),
                QueryParameters.GetQueryParameters(productType),
                taskReportingController);
            pageResultsExtractingController = new AccountProductsExtractionController();
        }
    }
}
