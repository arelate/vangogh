using System.Linq;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Status;

namespace GOG.Activities.Cleanup
{
    public class CleanupUpdatedActivity : Activity
    {
        private IDataController<long> updatedDataController;

        public CleanupUpdatedActivity(
            IDataController<long> updatedDataController,
            IStatusController statusController) :
            base(statusController)
        {
            this.updatedDataController = updatedDataController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var cleanupUpdatedTask = await statusController.CreateAsync(status, "Cleanup updated, close update cycle");

            await updatedDataController.RemoveAsync(
                cleanupUpdatedTask,
                (await updatedDataController.ItemizeAllAsync(cleanupUpdatedTask)).ToArray());

            await statusController.CompleteAsync(cleanupUpdatedTask);
        }
    }
}
