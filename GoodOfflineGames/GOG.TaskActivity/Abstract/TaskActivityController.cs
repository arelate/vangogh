using System;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.TaskActivity;

namespace GOG.TaskActivities.Abstract
{
    public abstract class TaskActivityController: ITaskActivityController
    {
        protected ITaskReportingController taskReportingController;

        public TaskActivityController(ITaskReportingController taskReportingController)
        {
            this.taskReportingController = taskReportingController;
        }

        public virtual Task ProcessTaskAsync()
        {
            throw new NotImplementedException();
        }
    }
}
