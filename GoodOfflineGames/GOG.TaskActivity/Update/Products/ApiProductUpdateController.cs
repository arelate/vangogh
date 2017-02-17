using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.UpdateDependencies;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.TaskActivities.Update.Products
{
    public class ApiProductUpdateController : ProductCoreUpdateController<ApiProduct, Product>
    {
        public ApiProductUpdateController(
            IDataController<ApiProduct> apiProductDataController,
            IDataController<Product> productsDataController,
            IDataController<long> updatedDataController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IUpdateUriController updateUriController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                ProductTypes.ApiProduct,
                apiProductDataController,
                productsDataController,
                updatedDataController,
                networkController,
                serializationController,
                null, // throttleController
                updateUriController,
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
