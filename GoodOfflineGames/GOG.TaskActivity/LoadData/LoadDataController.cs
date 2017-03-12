using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.TaskStatus;

namespace GOG.TaskActivities.Load
{
    public class LoadDataController: TaskActivityController
    {
        private ILoadDelegate[] loadDelegates;

        public LoadDataController(
            ITaskStatusController taskStatusController,
            params ILoadDelegate[] loadDelegates): 
            base(taskStatusController)
        {
            this.loadDelegates = loadDelegates;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var loadDataTask = taskStatusController.Create(taskStatus, "Load existing data");
            for (var ii = 0; ii < loadDelegates.Length; ii++)
            {
                taskStatusController.UpdateProgress(
                    loadDataTask, ii + 1,
                    loadDelegates.Length,
                    "Existing data");

                await loadDelegates[ii].LoadAsync();
            }
            taskStatusController.Complete(loadDataTask);
        }
    }
}
