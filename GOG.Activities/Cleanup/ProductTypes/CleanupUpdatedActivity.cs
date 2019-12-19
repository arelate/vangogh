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
        IActionLogController actionLogController;

        public CleanupUpdatedActivity(
            IDataController<long> updatedDataController,
            IActionLogController actionLogController)
        {
            this.updatedDataController = updatedDataController;
            this.actionLogController = actionLogController;
        }

        public Task ProcessActivityAsync()
        {
            // TODO: cleanup updated needs to use records controller
            throw new System.NotImplementedException();
        }
    }
}
