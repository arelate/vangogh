using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.Data;

using GOG.Models;

using GOG.TaskActivities.Abstract;

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
            ITaskReportingController taskReportingController):
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
                taskReportingController)
        {
            // ...
        }
    }
}
