using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Activities;
using GOG.Interfaces.Delegates.Authorize;
using Attributes;

namespace GOG.Delegates.Respond.Authorize
{
    [RespondsToRequests(Method = "authorize")]
    public class RespondToAuthorizeRequestDelegate : IRespondAsyncDelegate
    {
        private readonly IAuthorizeAsyncDelegate authorizeAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            "GOG.Delegates.Authorize.AuthorizeAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToAuthorizeRequestDelegate(
            IAuthorizeAsyncDelegate authorizeAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.authorizeAsyncDelegate = authorizeAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            var username = string.Empty;
            var password = string.Empty;

            await authorizeAsyncDelegate.AuthorizeAsync(
                username,
                password);
        }
    }
}