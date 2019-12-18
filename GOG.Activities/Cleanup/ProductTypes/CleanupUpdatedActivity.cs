using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;
using Interfaces.Activity;

using Attributes;

namespace GOG.Activities.Cleanup.ProductTypes
{
    public class CleanupUpdatedActivity : IActivity
    {
        IDataController<long> updatedDataController;
        IResponseLogController responseLogController;

        public CleanupUpdatedActivity(
            IDataController<long> updatedDataController,
            IResponseLogController responseLogController)
        {
            this.updatedDataController = updatedDataController;
            this.responseLogController = responseLogController;
        }

        public Task ProcessActivityAsync()
        {
            // TODO: cleanup updated needs to use records controller
            throw new System.NotImplementedException();
        }
    }
}
