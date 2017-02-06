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
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IUpdateUriController updateUriController,
            ISkipUpdateController skipUpdateController,
            IDataDecodingController dataDecodingController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController):
            base(
                ProductTypes.GameProductData,
                gameProductDataController,
                productsDataController,
                networkController,
                serializationController,
                null, // throttleController
                updateUriController,
                null, // requiredUpdatesController
                skipUpdateController,
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
