using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Respond;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Authorization;
using Attributes;
using Delegates.Activities;

namespace GOG.Delegates.Respond.Authorize
{
    [RespondsToRequests(Method = "authorize")]
    public class RespondToAuthorizeRequestDelegate : IRespondAsyncDelegate
    {
        private readonly IAuthorizeAsyncDelegate authorizeAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(GOG.Delegates.Authorize.AuthorizeAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
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