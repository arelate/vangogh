using System.Threading.Tasks;

using Interfaces.Settings;
using Interfaces.Status;

using GOG.Interfaces.Authorization;

namespace GOG.Activities.Authorize
{
    public class AuthorizeActivity : Activity
    {
        private IGetSettingsAsyncDelegate getSettingsAsyncDelegate;
        private IAuthorizationController authorizationController;

        public AuthorizeActivity(
            IGetSettingsAsyncDelegate getSettingsAsyncDelegate,
            IAuthorizationController authorizationController,
            IStatusController statusController) :
            base(statusController)
        {
            this.getSettingsAsyncDelegate = getSettingsAsyncDelegate;
            this.authorizationController = authorizationController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var settings = await getSettingsAsyncDelegate.GetSettingsAsync(status);
            if (settings != null)
            {
                await authorizationController.AuthorizeAsync(
                    settings.Username,
                    settings.Password,
                    status);
            }
            else throw new System.ArgumentNullException("Settings are null");
        }
    }
}
