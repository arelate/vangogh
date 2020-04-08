using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Controllers.Authorization;
using Attributes;

namespace GOG.Delegates.Respond.Authorize
{
    [RespondsToRequests(Method = "authorize")]
    public class RespondToAuthorizeRequestDelegate : IRespondAsyncDelegate
    {
        private readonly IAuthorizationController authorizationController;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "GOG.Controllers.Authorization.GOGAuthorizationController,GOG.Controllers",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToAuthorizeRequestDelegate(
            IAuthorizationController authorizationController,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.authorizationController = authorizationController;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
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