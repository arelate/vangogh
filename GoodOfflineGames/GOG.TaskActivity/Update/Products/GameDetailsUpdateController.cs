using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.Connection;
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
            IThrottleController throttleController,
            IUpdateUriController updateUriController,
            IConnectDelegate<GameDetails, AccountProduct> accountProductGameDetailsConnectDelegate,
            IAdditionalDetailsController additionalDetailsController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                ProductTypes.GameDetails,
                gameDetailsDataController,
                accountProductsDataController,
                updatedDataController,
                getGameDetailsDelegate,
                throttleController,
                updateUriController,
                accountProductGameDetailsConnectDelegate,
                additionalDetailsController,
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
