using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Respond;

using Interfaces.Controllers.Logs;

using GOG.Interfaces.Controllers.Authorization;

using Attributes;

namespace GOG.Delegates.Respond.Authorize
{
    [RespondsToRequests(Method="authorize")]
    public class RespondToAuthorizeRequestDelegate : IRespondAsyncDelegate
    {
        readonly IAuthorizationController authorizationController;
        readonly IActionLogController actionLogController;

        [Dependencies(
            "GOG.Controllers.Authorization.GOGAuthorizationController,GOG.Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public RespondToAuthorizeRequestDelegate(
            IAuthorizationController authorizationController,
            IActionLogController actionLogController)
        {
            this.authorizationController = authorizationController;
            this.actionLogController = actionLogController;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            var username = string.Empty;
            var password = string.Empty;

            await authorizationController.AuthorizeAsync(
                username,
                password);
        }
    }
}
