using System.Threading.Tasks;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Logs;
using Interfaces.Activity;


using GOG.Interfaces.Controllers.Authorization;

using Attributes;

using Models.Settings;

namespace GOG.Activities.Authorize
{
    public class AuthorizeActivity : IActivity
    {
        readonly IAuthorizationController authorizationController;
        readonly IActionLogController actionLogController;

        [Dependencies(
            "GOG.Controllers.Authorization.GOGAuthorizationController,GOG.Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public AuthorizeActivity(
            IAuthorizationController authorizationController,
            IActionLogController actionLogController)
        {
            this.authorizationController = authorizationController;
            this.actionLogController = actionLogController;
        }

        public async Task ProcessActivityAsync()
        {

            var username = string.Empty;
            var password = string.Empty;

            await authorizationController.AuthorizeAsync(
                username,
                password);
        }
    }
}
