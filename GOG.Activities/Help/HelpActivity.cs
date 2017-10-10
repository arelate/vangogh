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
        private IConsoleController consoleController;

        public HelpActivity(
            IActivityContextController activityContextController,
            IConsoleController consoleController,
            IStatusController statusController) : 
            base(statusController)
        {
            this.activityContextController = activityContextController;
            this.consoleController = consoleController;
        }

        public override Task ProcessActivityAsync(IStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
