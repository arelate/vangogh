using System;
using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.ActivityContext;

namespace GOG.Activities.Help
{
    public class HelpActivity : Activity
    {
        IActivityContextController activityContextController;

        public HelpActivity(
            IActivityContextController activityContextController,
            IStatusController statusController) : 
            base(statusController)
        {
            this.activityContextController = activityContextController;
        }

        public override Task ProcessActivityAsync(IStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
