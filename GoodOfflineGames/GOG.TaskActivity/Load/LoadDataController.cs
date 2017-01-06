using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Load
{
    public class LoadDataController: TaskActivityController
    {
        private ILoadDelegate[] loadDelegates;

        public LoadDataController(
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController,
            //ITaskReportingController taskReportingController,
            params ILoadDelegate[] loadDelegates): 
            base(
                taskStatus,
                taskStatusController)
        {
            this.loadDelegates = loadDelegates;
        }

        public override async Task ProcessTaskAsync()
        {
            var loadDataTask = taskStatusController.Create(taskStatus, "Load existing data");
            foreach (var loadDelegate in loadDelegates)
                await loadDelegate.LoadAsync();
            taskStatusController.Complete(loadDataTask);
        }
    }
}
