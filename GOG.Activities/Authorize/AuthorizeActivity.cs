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
        readonly IResponseLogController responseLogController;

        [Dependencies(
            "GOG.Controllers.Authorization.GOGAuthorizationController,GOG.Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public AuthorizeActivity(
            IAuthorizationController authorizationController,
            IResponseLogController responseLogController)
        {
            this.authorizationController = authorizationController;
            this.responseLogController = responseLogController;
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
