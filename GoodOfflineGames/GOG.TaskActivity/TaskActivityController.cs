using System;
using System.Threading.Tasks;

using Interfaces.TaskActivity;
using Interfaces.TaskStatus;

namespace GOG.TaskActivities
{
    public abstract class TaskActivityController: ITaskActivityController
    {
        protected ITaskStatusController taskStatusController;

        public TaskActivityController(
            ITaskStatusController taskStatusController)
        {
            this.taskStatusController = taskStatusController;
        }

        public virtual Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            throw new NotImplementedException();
        }
    }
}
