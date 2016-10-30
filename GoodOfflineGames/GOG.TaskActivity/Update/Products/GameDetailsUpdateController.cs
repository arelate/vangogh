using Interfaces.Reporting;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Throttle;
using Interfaces.UpdateDependencies;
using Interfaces.AdditionalDetails;
using Interfaces.Data;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Products
{
    public class GameDetailsUpdateController : ProductCoreUpdateController<GameDetails, AccountProduct>
    {
        public GameDetailsUpdateController(
            IDataController<GameDetails> gameDetailsDataController,
            IDataController<AccountProduct> accountProductsDataController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IThrottleController throttleController,
            IUpdateUriController updateUriController,
            IRequiredUpdatesController requiredUpdatesController,
            IConnectionController connectionController,
            IAdditionalDetailsController additionalDetailsController,
            ITaskReportingController taskReportingController) :
            base(
                ProductTypes.GameDetails,
                gameDetailsDataController,
                accountProductsDataController,
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
            // ...
        }
    }
}
