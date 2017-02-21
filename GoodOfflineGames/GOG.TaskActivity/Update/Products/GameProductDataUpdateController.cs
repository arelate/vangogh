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
            IGetDeserializedDelegate<GameProductData> getGameProductDataDelegate,
            IUpdateUriController updateUriController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController):
            base(
                ProductTypes.GameProductData,
                gameProductDataController,
                productsDataController,
                updatedDataController,
                getGameProductDataDelegate,
                null, // throttleController
                updateUriController,
                null, // connectionController
                null, // additionalDetailsController
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
