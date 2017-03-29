using System;
using System.Linq;
using System.Collections.Generic;

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
        private const int requestIntervalSeconds = 2 * 60;

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

            this.uriPrefixes = uriPrefixes;

            if (this.uriPrefixes != null)
                foreach (var prefix in this.uriPrefixes)
                    lastRequestToUriPrefix.Add(
                        prefix, 
                        DateTime.UtcNow - TimeSpan.FromSeconds(requestIntervalSeconds));
        }

        public void EnforceRequestRate(string uri, IStatus status)
        {
            var prefix = collectionController.Reduce(uriPrefixes, p => uri.StartsWith(p)).SingleOrDefault();
            if (string.IsNullOrEmpty(prefix)) return;

            var now = DateTime.UtcNow;
            var elapsed = (int) (now - lastRequestToUriPrefix[prefix]).TotalSeconds;
            if (elapsed < requestIntervalSeconds)
            {
                var limitRateTask = statusController.Create(status, "Limit request rate to avoid temporary server block");
                throttleController.Throttle(requestIntervalSeconds - elapsed, status);
                statusController.Complete(limitRateTask);
            }

            lastRequestToUriPrefix[prefix] = now;
        }
    }
}
