using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.UpdateDependencies;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.TaskActivities.Update.Products
{
    public class GameProductDataUpdateController: ProductCoreUpdateController<GameProductData, Product>
    {
        public GameProductDataUpdateController(
            IDataController<GameProductData> gameProductDataController,
            IDataController<Product> productsDataController,
            IDataController<long> updatedDataController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IUpdateUriController updateUriController,
            IDataDecodingController dataDecodingController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController):
            base(
                ProductTypes.GameProductData,
                gameProductDataController,
                productsDataController,
                updatedDataController,
                networkController,
                serializationController,
                null, // throttleController
                updateUriController,
                dataDecodingController,
                null, // connectionController
                null, // additionalDetailsController
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
