using System.Threading.Tasks;

using Interfaces.Settings;
using Interfaces.TaskStatus;

using GOG.Interfaces.Authorization;

namespace GOG.Activities.Authorize
{
    public class AuthorizeActivity : Activity
    {
        private ISettingsProperty settingsProperty;
        private IAuthorizationController authorizationController;

        public AuthorizeActivity(
            ISettingsProperty settingsProperty,
            IAuthorizationController authorizationController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.settingsProperty = settingsProperty;
            this.authorizationController = authorizationController;
        }

        public override async Task ProcessActivityAsync(ITaskStatus taskStatus)
        {
            var authorizationTask = taskStatusController.Create(taskStatus, "Authorize on GOG.com");

            if (settingsProperty != null &&
                settingsProperty.Settings != null)
            {
                await authorizationController.Authorize(
                    settingsProperty.Settings.Username,
                    settingsProperty.Settings.Password,
                    authorizationTask);
            }

            taskStatusController.Complete(authorizationTask);
        }
    }
}
