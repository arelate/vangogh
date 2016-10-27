using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.AdditionalDetails;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Products
{
    public class GameDetailsUpdateController : ProductCoreUpdateController<GameDetails, AccountProduct>
    {
        public GameDetailsUpdateController(
            //IProductTypeStorageController productStorageController,
            ICollectionController collectionController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IThrottleController throttleController,
            IUpdateUriController updateUriController,
            IRequiredUpdatesController requiredUpdatesController,
            IConnectionController connectionController,
            IAdditionalDetailsController additionalDetailsController,
            ITaskReportingController taskReportingController) :
            base(
                //productStorageController,
                collectionController,
                networkController,
                serializationController,
                throttleController,
                updateUriController,
                requiredUpdatesController,
                null, // skipUpdateController
                null, // dataDecodingController
                connectionController,
                additionalDetailsController,
                taskReportingController)
        {
            updateProductType = ProductTypes.GameDetails;
            listProductType = ProductTypes.AccountProduct;

            displayProductName = "game details";
        }
    }
}
