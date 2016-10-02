using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Settings;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Console;
using Interfaces.Serialization;
using Interfaces.PropertiesValidation;

using GOG.Interfaces.Authorization;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Authorization
{
    public class AuthorizationController : TaskActivityController
    {

        private IUriController uriController;
        private INetworkController networkController;
        private ISerializationController<string> serializationController;
        private IExtractionController loginTokenExtractionController;
        private IExtractionController loginIdExtractionController;
        private IConsoleController consoleController;
        private IAuthenticationProperties authenticateProperties;
        private IAuthenticationPropertiesValidationController authenticationPropertiesValidationController;
        private IAuthorizationController authorizationController;

        public AuthorizationController(
            IUriController uriController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IExtractionController loginTokenExtractionController,
            IExtractionController loginIdExtractionController,
            IConsoleController consoleController,
            IAuthenticationProperties authenticateProperties,
            IAuthenticationPropertiesValidationController  authenticationPropertiesValidationController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.authenticateProperties = authenticateProperties;
            this.authenticationPropertiesValidationController = authenticationPropertiesValidationController;
            this.uriController = uriController;
            this.networkController = networkController;
            this.serializationController = serializationController;
            this.loginTokenExtractionController = loginTokenExtractionController;
            this.loginIdExtractionController = loginIdExtractionController;
            this.consoleController = consoleController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Authorization on GOG.com");

            authorizationController = new Controllers.Authorization.AuthorizationController(
                authenticationPropertiesValidationController,
                uriController,
                networkController,
                serializationController,
                loginTokenExtractionController,
                loginIdExtractionController,
                consoleController);

            await authorizationController.Authorize(authenticateProperties);

            taskReportingController.CompleteTask();
        }
    }
}
