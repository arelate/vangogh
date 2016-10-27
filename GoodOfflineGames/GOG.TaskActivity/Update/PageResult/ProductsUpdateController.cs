using Interfaces.Reporting;
using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.ProductTypes;
using Interfaces.Products;

using Models.Uris;
using Models.QueryParameters;

using GOG.Models;

using GOG.Controllers.PageResults;
using GOG.Controllers.Extraction;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.PageResult
{
    public class ProductsUpdateController : PageResultUpdateController<ProductsPageResult, Product>
    {
        public ProductsUpdateController(
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            IProductsController<Product> productsController,
            ITaskReportingController taskReportingController) :
            base(requestPageController,
                productsController,
                taskReportingController)
        {
            productType = ProductTypes.Product;
            pageResultsController = new ProductsPageResultController(
                requestPageController,
                serializationController,
                Uris.Paths.GetUpdateUri(productType),
                QueryParameters.GetQueryParameters(productType),
                taskReportingController);
            pageResultsExtractingController = new ProductsExtractionController();
        }
    }
}
