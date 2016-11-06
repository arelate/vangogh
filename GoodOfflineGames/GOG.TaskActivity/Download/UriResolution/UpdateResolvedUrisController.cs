using System.Threading.Tasks;

using Interfaces.Reporting;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Download.UriResolution
{
    public class UpdateResolvedUrisController: TaskActivityController
    {
        public UpdateResolvedUrisController(
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            // ...
        }

        public override Task ProcessTask()
        {
            return base.ProcessTask();
        }
    }
}
