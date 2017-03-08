using System.Threading.Tasks;

using Interfaces.Settings;
using Interfaces.Uri;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Console;
using Interfaces.Serialization;
using Interfaces.PropertiesValidation;
using Interfaces.TaskStatus;

using GOG.Interfaces.Authorization;

namespace GOG.TaskActivities.Authorization
{
    public class AuthorizationController : TaskActivityController
    {

        private IUriController uriController;
        private INetworkController networkController;
        private ISerializationController<string> serializationController;
        private IStringExtractionController loginTokenExtractionController;
        private IStringExtractionController loginIdExtractionController;
        private IStringExtractionController loginUsernameExtractionController;
        private IConsoleController consoleController;
        private IAuthenticationProperties authenticateProperties;
        private IAuthenticationPropertiesValidationController authenticationPropertiesValidationController;
        private IAuthorizationController authorizationController;

        public AuthorizationController(
            IUriController uriController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IStringExtractionController loginTokenExtractionController,
            IStringExtractionController loginIdExtractionController,
            IStringExtractionController loginUsernameExtractionController,
            IConsoleController consoleController,
            IAuthenticationProperties authenticateProperties,
            IAuthenticationPropertiesValidationController  authenticationPropertiesValidationController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.authenticateProperties = authenticateProperties;
            this.authenticationPropertiesValidationController = authenticationPropertiesValidationController;
            this.uriController = uriController;
            this.networkController = networkController;
            this.serializationController = serializationController;
            this.loginTokenExtractionController = loginTokenExtractionController;
            this.loginIdExtractionController = loginIdExtractionController;
            this.loginUsernameExtractionController = loginUsernameExtractionController;
            this.consoleController = consoleController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var authorizationTask = taskStatusController.Create(taskStatus, "Authorize on GOG.com");

            authorizationController = new Controllers.Authorization.AuthorizationController(
                authenticationPropertiesValidationController,
                uriController,
                networkController,
                serializationController,
                loginTokenExtractionController,
                loginIdExtractionController,
                loginUsernameExtractionController,
                consoleController);

            await authorizationController.Authorize(authenticateProperties);

            taskStatusController.Complete(authorizationTask);
        }
    }
}
