using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.UpdateUri;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.Models;

using Models.ProductCore;

namespace GOG.TaskActivities.Update.Products
{
    public class ApiProductUpdateController : ProductCoreUpdateController<ApiProduct, Product>
    {
        public ApiProductUpdateController(
            IDataController<ApiProduct> apiProductDataController,
            IDataController<Product> productsDataController,
            IDataController<long> updatedDataController,
            IGetDeserializedDelegate<ApiProduct> getApiProductDelegate,
            IGetUpdateUriDelegate<Product> getProductUpdateUriDelegate,
            ITaskStatusController taskStatusController) :
            base(
                ProductTypes.ApiProduct,
                apiProductDataController,
                productsDataController,
                updatedDataController,
                getApiProductDelegate,
                getProductUpdateUriDelegate,
                taskStatusController)
        {
            // ...
        }
    }
}
