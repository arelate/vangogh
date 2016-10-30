using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Data;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Load
{
    public class LoadDataController: TaskActivityController
    {
        private ILoadDelegate[] loadDelegates;

        public LoadDataController(
            ITaskReportingController taskReportingController,
            params ILoadDelegate[] loadDelegates): 
            base(taskReportingController)

        {
            this.loadDelegates = loadDelegates;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Load existing data");
            foreach (var loadDelegate in loadDelegates)
                await loadDelegate.Load();
            taskReportingController.CompleteTask();
        }
    }
}
