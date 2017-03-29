using System.Linq;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.TaskStatus;

namespace GOG.Activities.Cleanup
{
    public class CleanupUpdatedActivity : Activity
    {
        private IDataController<long> updatedDataController;

        public CleanupUpdatedActivity(
            IDataController<long> updatedDataController,
            ITaskStatusController taskStatusController): 
            base(taskStatusController)
        {
            this.updatedDataController = updatedDataController;
        }

        public override async Task ProcessActivityAsync(ITaskStatus taskStatus)
        {
            var cleanupUpdatedTask = taskStatusController.Create(taskStatus, "Cleanup updated, close update cycle");

            await updatedDataController.RemoveAsync(
                cleanupUpdatedTask, 
                updatedDataController.EnumerateIds().ToArray());

            taskStatusController.Complete(cleanupUpdatedTask);
        }
    }
}
