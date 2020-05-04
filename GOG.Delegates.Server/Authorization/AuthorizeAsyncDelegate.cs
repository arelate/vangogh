using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Server;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Authorization;
using Attributes;
using Delegates.Activities;

namespace GOG.Delegates.Server.Authorization
{
    [RespondsToRequests(Method = "authorize")]
    public class AuthorizeAsyncDelegate : IProcessAsyncDelegate
    {
        private readonly IAuthorizeAsyncDelegate authorizeAsyncDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;

        [Dependencies(
            typeof(Delegates.Authorization.AuthorizeAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public AuthorizeAsyncDelegate(
            IAuthorizeAsyncDelegate authorizeAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.authorizeAsyncDelegate = authorizeAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task ProcessAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            var username = string.Empty;
            var password = string.Empty;

            await authorizeAsyncDelegate.AuthorizeAsync(
                username,
                password);
        }
    }
}