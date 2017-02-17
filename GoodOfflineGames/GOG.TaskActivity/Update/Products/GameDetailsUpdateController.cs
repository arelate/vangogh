using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.AdditionalDetails;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.TaskActivities.Update.Products
{
    public class GameDetailsUpdateController : ProductCoreUpdateController<GameDetails, AccountProduct>
    {
        public GameDetailsUpdateController(
            IDataController<GameDetails> gameDetailsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IDataController<long> updatedDataController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IThrottleController throttleController,
            IUpdateUriController updateUriController,
            IConnectionController connectionController,
            IAdditionalDetailsController additionalDetailsController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                ProductTypes.GameDetails,
                gameDetailsDataController,
                accountProductsDataController,
                updatedDataController,
                networkController,
                serializationController,
                throttleController,
                updateUriController,
                null, // dataDecodingController
                connectionController,
                additionalDetailsController,
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
