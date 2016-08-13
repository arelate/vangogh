using System.Collections.Generic;

using Interfaces.Reporting;
using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.ProductTypes;
using Interfaces.Storage;

using Models.Uris;
using Models.QueryParameters;

using GOG.Models;

using GOG.Controllers.PageResults;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.ProductsUpdate
{
    public class ProductsUpdateController : UpdateController<ProductsPageResult, Product>
    {
        public ProductsUpdateController(
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            IProductTypeStorageController productStorageController,
            ITaskReportingController taskReportingController) :
            base(requestPageController,
                serializationController,
                productStorageController,
                taskReportingController)
        {
            productType = ProductTypes.Product;
            pageResultsController = new ProductsPageResultController(
                requestPageController,
                serializationController,
                Uris.Paths.GetUpdateUri(productType),
                QueryParameters.GetQueryParameters(productType),
                taskReportingController);
            pageResultsExtractingController = new ProductsPageResultsExtractingController();
        }
    }
}
