using System.Threading.Tasks;

using Interfaces.Settings;
using Interfaces.Status;

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
            IStatusController statusController) :
            base(statusController)
        {
            this.settingsProperty = settingsProperty;
            this.authorizationController = authorizationController;
        }

        public override async Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            if (settingsProperty != null &&
                settingsProperty.Settings != null)
            {
                await authorizationController.Authorize(
                    settingsProperty.Settings.Username,
                    settingsProperty.Settings.Password,
                    status);
            }
            else throw new System.ArgumentNullException("Settings are null");
        }
    }
}
