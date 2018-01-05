using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.RequestRate;
using Interfaces.Throttle;
using Interfaces.Status;
using Interfaces.Collection;

namespace Controllers.RequestRate
{
    public class RequestRateController : IRequestRateController
    {
        private IThrottleController throttleController;
        private ICollectionController collectionController;
        private IStatusController statusController;
        private Dictionary<string, DateTime> lastRequestToUriPrefix;
        private string[] uriPrefixes;
        private const int requestIntervalSeconds = 30;
        private const int passthroughCount = 100; // don't throttle first N requests
        private int rateLimitRequestsCount;

        public RequestRateController(
            IThrottleController throttleController,
            ICollectionController collectionController,
            IStatusController statusController,
            params string[] uriPrefixes)
        {
            this.throttleController = throttleController;
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

        public async Task EnforceRequestRateAsync(string uri, IStatus status)
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
                await throttleController.ThrottleAsync(requestIntervalSeconds - elapsed, status);
                await statusController.CompleteAsync(limitRateTask);
            }

            lastRequestToUriPrefix[prefix] = now;
        }
    }
}
