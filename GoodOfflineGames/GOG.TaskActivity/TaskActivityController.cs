using System;
using System.Threading.Tasks;

using Interfaces.Activity;
using Interfaces.TaskStatus;

namespace GOG.Activities
{
    public abstract class Activity: IActivity
    {
        protected ITaskStatusController taskStatusController;

        public Activity(
            ITaskStatusController taskStatusController)
        {
            this.taskStatusController = taskStatusController;
        }

        public virtual Task ProcessActivityAsync(ITaskStatus taskStatus)
        {
            throw new NotImplementedException();
        }
    }
}
