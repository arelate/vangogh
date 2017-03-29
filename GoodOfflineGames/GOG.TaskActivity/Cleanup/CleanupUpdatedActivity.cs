using System.Linq;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.TaskStatus;

namespace GOG.TaskActivities.Cleanup
{
    public class CleanupUpdatedActivity : TaskActivityController
    {
        private IDataController<long> updatedDataController;

        public CleanupUpdatedActivity(
            IDataController<long> updatedDataController,
            ITaskStatusController taskStatusController): 
            base(taskStatusController)
        {
            this.updatedDataController = updatedDataController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var cleanupUpdatedTask = taskStatusController.Create(taskStatus, "Cleanup updated, close update cycle");

            await updatedDataController.RemoveAsync(
                cleanupUpdatedTask, 
                updatedDataController.EnumerateIds().ToArray());

            taskStatusController.Complete(cleanupUpdatedTask);
        }
    }
}
