using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.UpdateDependencies;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Products
{
    public class ApiProductUpdateController : ProductCoreUpdateController<ApiProduct, Product>
    {
        public ApiProductUpdateController(
            IDataController<ApiProduct> apiProductDataController,
            IDataController<Product> productsDataController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IUpdateUriController updateUriController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                ProductTypes.ApiProduct,
                apiProductDataController,
                productsDataController,
                networkController,
                serializationController,
                null, // throttleController
                updateUriController,
                null, // requiredUpdatesController
                null, // skipUpdateController
                null, // dataDecodingController
                null, // connectionController
                null, // additionalDetailsController
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
