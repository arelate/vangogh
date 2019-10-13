using System.Threading.Tasks;

using Interfaces.Controllers.Stash;
using Interfaces.Status;

using GOG.Interfaces.Controllers.Authorization;

using Models.Settings;

namespace GOG.Activities.Authorize
{
    public class AuthorizeActivity : Activity
    {
        readonly IAuthorizationController authorizationController;

        public AuthorizeActivity(
            IAuthorizationController authorizationController,
            IStatusController statusController) :
            base(statusController)
        {
            this.authorizationController = authorizationController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {

            var username = string.Empty;
            var password = string.Empty;

            await authorizationController.AuthorizeAsync(
                username,
                password,
                status);
        }
    }
}
