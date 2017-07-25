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

        public async override Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            statusController.AddSummaryResults(status, "Syntax: GoodOfflineGames.exe [activity] [parameters]");
            statusController.AddSummaryResults(status, "Supported activities:");

            foreach (var activity in ActivityContext.Whitelist.Keys)
                foreach (var context in ActivityContext.Whitelist[activity])
                {
                    var activityContext = activityContextController.ToString((activity, context)).ToLower();
                    statusController.AddSummaryResults(
                        status,
                        activityContext);
                }
        }
    }
}
