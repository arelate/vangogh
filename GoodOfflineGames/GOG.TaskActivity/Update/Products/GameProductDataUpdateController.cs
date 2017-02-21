using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.UpdateUri;
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
            IGetUpdateUriDelegate<Product> getProductUpdateUriDelegate,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController):
            base(
                ProductTypes.GameProductData,
                gameProductDataController,
                productsDataController,
                updatedDataController,
                getGameProductDataDelegate,
                getProductUpdateUriDelegate,
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
