using System.Threading.Tasks;

using Interfaces.Controllers.Stash;
using Interfaces.Status;

using GOG.Interfaces.Controllers.Authorization;

using Models.Settings;

namespace GOG.Activities.Authorize
{
    public class AuthorizeActivity : Activity
    {
        private IGetDataAsyncDelegate<Settings> getSettingsDataAsyncDelegate;
        private IAuthorizationController authorizationController;

        public AuthorizeActivity(
            IGetDataAsyncDelegate<Settings> getSettingsDataAsyncDelegate,
            IAuthorizationController authorizationController,
            IStatusController statusController) :
            base(statusController)
        {
            this.getSettingsDataAsyncDelegate = getSettingsDataAsyncDelegate;
            this.authorizationController = authorizationController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var settings = await getSettingsDataAsyncDelegate.GetDataAsync(status);
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
