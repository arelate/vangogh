using System.Threading.Tasks;

using Interfaces.Status;
using Interfaces.Console;

namespace GOG.Activities.Help
{
    public class HelpActivity : Activity
    {
        private IConsoleController consoleController;

        public HelpActivity(
            IConsoleController consoleController,
            IStatusController statusController) : 
            base(statusController)
        {
            this.consoleController = consoleController;
        }

        public async override Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            await base.ProcessActivityAsync(status);
        }
    }
}
