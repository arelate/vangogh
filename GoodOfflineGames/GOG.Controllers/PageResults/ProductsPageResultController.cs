using System.Collections.Generic;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.TaskStatus;
using Interfaces.ProductTypes;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class ProductsPageResultController : PageResultsController<ProductsPageResult>
    {
        public ProductsPageResultController(
            ProductTypes productType,
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            ITaskStatusController taskStatusController) : 
            base(
                productType,
                requestPageController, 
                serializationController, 
                taskStatusController)
        {
            // ...
        }
    }
}
