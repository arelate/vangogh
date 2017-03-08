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
            foreach (var loadDelegate in loadDelegates)
                await loadDelegate.LoadAsync();
            taskStatusController.Complete(loadDataTask);
        }
    }
}
