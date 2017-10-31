using System;
using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Console;
using Interfaces.ActivityContext;

using Models.ActivityContext;

namespace GOG.Activities.Help
{
    public class HelpActivity : Activity
    {
        private IActivityContextController activityContextController;

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
