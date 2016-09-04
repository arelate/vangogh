using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Politeness;
using Interfaces.UpdateDependencies;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Products
{
    public class GameProductDataUpdateController: ProductUpdateController<GameProductData, Product>
    {
        public GameProductDataUpdateController(
            IProductTypeStorageController productStorageController,
            ICollectionController collectionController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IPolitenessController politenessController,
            IUpdateUriController updateUriController,
            ISkipUpdateController skipUpdateController,
            IDataDecodingController dataDecodingController,
            ITaskReportingController taskReportingController):
            base(productStorageController,
                collectionController,
                networkController,
                serializationController,
                politenessController,
                updateUriController,
                null, // requiredUpdatesController
                skipUpdateController,
                dataDecodingController,
                null, // connectionController
                taskReportingController)
        {
            updateProductType = ProductTypes.GameProductData;
            listProductType = ProductTypes.Product;

            displayProductName = "game product data";
        }
    }
}
