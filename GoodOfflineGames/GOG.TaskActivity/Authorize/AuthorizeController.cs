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

namespace GOG.TaskActivities.Authorize
{
    public class AuthorizeController : TaskActivityController
    {
        private string username;
        private string password;
        //private IUriController uriController;
        //private INetworkController networkController;
        //private ISerializationController<string> serializationController;
        //private IStringExtractionController loginTokenExtractionController;
        //private IStringExtractionController loginIdExtractionController;
        //private IStringExtractionController loginUsernameExtractionController;
        //private IConsoleController consoleController;
        //private IValidatePropertiesDelegate<string> usernamePasswordValidationDelegate;
        private IAuthorizationController authorizationController;

        public AuthorizeController(
            string username,
            string password,
            IAuthorizationController authorizationController,
            //IUriController uriController,
            //INetworkController networkController,
            //ISerializationController<string> serializationController,
            //IStringExtractionController loginTokenExtractionController,
            //IStringExtractionController loginIdExtractionController,
            //IStringExtractionController loginUsernameExtractionController,
            //IConsoleController consoleController,
            //IValidatePropertiesDelegate<string> usernamePasswordValidationDelegate,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.username = username;
            this.password = password;

            this.authorizationController = authorizationController;

            //this.usernamePasswordValidationDelegate = usernamePasswordValidationDelegate;
            //this.uriController = uriController;
            //this.networkController = networkController;
            //this.serializationController = serializationController;
            //this.loginTokenExtractionController = loginTokenExtractionController;
            //this.loginIdExtractionController = loginIdExtractionController;
            //this.loginUsernameExtractionController = loginUsernameExtractionController;
            //this.consoleController = consoleController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var authorizationTask = taskStatusController.Create(taskStatus, "Authorize on GOG.com");

            //authorizationController = new Controllers.Authorization.AuthorizationController(
            //    usernamePasswordValidationDelegate,
            //    uriController,
            //    networkController,
            //    serializationController,
            //    loginTokenExtractionController,
            //    loginIdExtractionController,
            //    loginUsernameExtractionController,
            //    consoleController);

            await authorizationController.Authorize(username, password);

            taskStatusController.Complete(authorizationTask);
        }
    }
}
