using System;
using System.Threading.Tasks;

using Interfaces.Activity;
using Interfaces.Status;

namespace GOG.Activities
{
    public abstract class Activity: IActivity
    {
        protected IStatusController statusController;

        public Activity(
            IStatusController statusController)
        {
            this.statusController = statusController;
        }

        public virtual Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
