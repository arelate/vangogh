using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Constrain;

using Interfaces.Controllers.Collection;

using Interfaces.Status;

namespace Delegates.Constrain
{
    public class ConstrainRequestRateAsyncDelegate : IConstrainAsyncDelegate<string>
    {
        private IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate;
        private ICollectionController collectionController;
        private IStatusController statusController;
        private Dictionary<string, DateTime> lastRequestToUriPrefix;
        private string[] uriPrefixes;
        private const int requestIntervalSeconds = 30;
        private const int passthroughCount = 100; // don't throttle first N requests
        private int rateLimitRequestsCount;

        public ConstrainRequestRateAsyncDelegate(
            IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate,
            ICollectionController collectionController,
            IStatusController statusController,
            params string[] uriPrefixes)
        {
            this.constrainExecutionAsyncDelegate = constrainExecutionAsyncDelegate;
            this.collectionController = collectionController;
            this.statusController = statusController;
            lastRequestToUriPrefix = new Dictionary<string, DateTime>();
            rateLimitRequestsCount = 0;

            this.uriPrefixes = uriPrefixes;

            if (this.uriPrefixes != null)
                foreach (var prefix in this.uriPrefixes)
                    lastRequestToUriPrefix.Add(
                        prefix, 
                        DateTime.UtcNow - TimeSpan.FromSeconds(requestIntervalSeconds));
        }

        public async Task ConstrainAsync(string uri, IStatus status)
        {
            var prefix = collectionController.Reduce(uriPrefixes, p => uri.StartsWith(p)).SingleOrDefault();
            if (string.IsNullOrEmpty(prefix)) return;

            // don't limit rate for the first N requests, even if they match rate limit prefix
            if (++rateLimitRequestsCount <= passthroughCount) return;

            var now = DateTime.UtcNow;
            var elapsed = (int) (now - lastRequestToUriPrefix[prefix]).TotalSeconds;
            if (elapsed < requestIntervalSeconds)
            {
                var limitRateTask = await statusController.CreateAsync(status, "Limit request rate to avoid temporary server block");
                await constrainExecutionAsyncDelegate.ConstrainAsync(requestIntervalSeconds - elapsed, status);
                await statusController.CompleteAsync(limitRateTask);
            }

            lastRequestToUriPrefix[prefix] = now;
        }
    }
}
