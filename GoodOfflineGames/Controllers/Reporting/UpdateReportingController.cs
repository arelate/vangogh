using Interfaces.Console;
using Interfaces.Reporting;

namespace GoodOfflineGames
{
    public class UpdateReportingController : IUpdateReportingController
    {
        private IConsoleController consoleController;

        public UpdateReportingController(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public void ReportUpdate()
        {
            consoleController.Write(".", MessageType.Progress);
        }
    }
}
