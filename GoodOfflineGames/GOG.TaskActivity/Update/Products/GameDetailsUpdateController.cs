using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
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
            IGetDeserializedDelegate<GameDetails> getGameDetailsDelegate,
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
                getGameDetailsDelegate,
                serializationController,
                throttleController,
                updateUriController,
                connectionController,
                additionalDetailsController,
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
