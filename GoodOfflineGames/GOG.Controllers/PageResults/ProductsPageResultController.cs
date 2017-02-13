using System.Collections.Generic;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.TaskStatus;
using Interfaces.ProductTypes;
using Interfaces.ForEachAsync;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class ProductsPageResultController : PageResultsController<ProductsPageResult>
    {
        public ProductsPageResultController(
            ProductTypes productType,
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            IForEachAsyncDelegate forEachAsyncDelegate,
            ITaskStatusController taskStatusController) : 
            base(
                productType,
                requestPageController, 
                serializationController,
                forEachAsyncDelegate,
                taskStatusController)
        {
            // ...
        }
    }
}
