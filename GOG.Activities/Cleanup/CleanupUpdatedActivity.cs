using System.Linq;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Status;

namespace GOG.Activities.Cleanup
{
    public class CleanupUpdatedActivity : Activity
    {
        IDataController<long> updatedDataController;

        public CleanupUpdatedActivity(
            IDataController<long> updatedDataController,
            IStatusController statusController) :
            base(statusController)
        {
            this.updatedDataController = updatedDataController;
        }

        public override Task ProcessActivityAsync(IStatus status)
        {
            // TODO: cleanup updated needs to use records controller
            throw new System.NotImplementedException();
        }
    }
}
