using Interfaces.ProductTypes;
using Interfaces.Network;
using Interfaces.Throttle;
using Interfaces.UpdateUri;
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
            IGetUpdateUriDelegate<AccountProduct> getAccountProductUpdateUriDelegate,
            IConnectDelegate<GameDetails, AccountProduct> accountProductGameDetailsConnectDelegate,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) :
            base(
                ProductTypes.GameDetails,
                gameDetailsDataController,
                accountProductsDataController,
                updatedDataController,
                getGameDetailsDelegate,
                getAccountProductUpdateUriDelegate,
                taskStatus,
                taskStatusController,
                throttleController,
                accountProductGameDetailsConnectDelegate)
        {
            // ...
        }
    }
}
