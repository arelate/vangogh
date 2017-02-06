using System;
using System.Threading.Tasks;

using Interfaces.TaskActivity;
using Interfaces.TaskStatus;

namespace GOG.TaskActivities
{
    public abstract class TaskActivityController: ITaskActivityController
    {
        protected ITaskStatus taskStatus;
        protected ITaskStatusController taskStatusController;

        public TaskActivityController(
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController)
        {
            this.taskStatus = taskStatus;
            this.taskStatusController = taskStatusController;
        }

        public virtual Task ProcessTaskAsync()
        {
            throw new NotImplementedException();
        }
    }
}
