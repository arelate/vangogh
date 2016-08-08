using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Settings;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Console;

using GOG.Interfaces.Authorization;

using GOG.Controllers.Authorization;

namespace GOG.Controllers.TaskActivity
{
    public class AuthorizationTaskActivityController : TaskActivityController
    {

        private IUriController uriController;
        private INetworkController networkController;
        private IExtractionController extractionController;
        private IConsoleController consoleController;
        private IAuthenticateProperties authenticateProperties;
        private IAuthorizationController authorizationController;

        public AuthorizationTaskActivityController(
            IUriController uriController,
            INetworkController networkController,
            IExtractionController extractionController,
            IConsoleController consoleController,
            IAuthenticateProperties authenticateProperties,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.authenticateProperties = authenticateProperties;
            this.uriController = uriController;
            this.networkController = networkController;
            this.extractionController = extractionController;
            this.consoleController = consoleController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.AddTask("Authorization on GOG.com");

            authorizationController = new AuthorizationController(
                uriController,
                networkController,
                extractionController,
                consoleController);

            await authorizationController.Authorize(authenticateProperties);

            taskReportingController.CompleteTask();
        }
    }
}
