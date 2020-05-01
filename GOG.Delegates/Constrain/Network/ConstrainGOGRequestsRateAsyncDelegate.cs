using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.Constrain;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Itemize;
using Delegates.Constrain;
using Delegates.Collections.System;

namespace GOG.Delegates.Constrain.Network
{
    public class ConstrainGOGRequestRateAsyncDelegate : IConstrainAsyncDelegate<string>
    {
        private readonly IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate;
        private readonly IFindDelegate<string> findStringDelegate;
        private readonly Dictionary<string, DateTime> lastRequestToUriPrefix;
        private readonly IItemizeAllDelegate<string> itemizeRateContraindesUris;
        private const int requestIntervalSeconds = 30;
        private const int passthroughCount = 100; // don't throttle first N requests
        private int rateLimitRequestsCount;

        [Dependencies(
            typeof(ConstrainExecutionAsyncDelegate),
            typeof(FindStringDelegate),
            typeof(Itemize.ItemizeAllRateConstrainedUrisDelegate))]
        public ConstrainGOGRequestRateAsyncDelegate(
            IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate,
            IFindDelegate<string> findStringDelegate,
            IItemizeAllDelegate<string> itemizeRateContraindesUris)
        {
            this.constrainExecutionAsyncDelegate = constrainExecutionAsyncDelegate;
            this.findStringDelegate = findStringDelegate;
            lastRequestToUriPrefix = new Dictionary<string, DateTime>();
            rateLimitRequestsCount = 0;

            this.itemizeRateContraindesUris = itemizeRateContraindesUris;

            if (this.itemizeRateContraindesUris != null)
                foreach (var uri in this.itemizeRateContraindesUris.ItemizeAll())
                    lastRequestToUriPrefix.Add(
                        uri,
                        DateTime.UtcNow - TimeSpan.FromSeconds(requestIntervalSeconds));
        }

        public async Task ConstrainAsync(string uri)
        {
            var prefix = findStringDelegate.Find(itemizeRateContraindesUris.ItemizeAll(), uri.StartsWith);
            if (string.IsNullOrEmpty(prefix)) return;

            // don't limit rate for the first N requests, even if they match rate limit prefix
            if (++rateLimitRequestsCount <= passthroughCount) return;

            var now = DateTime.UtcNow;
            var elapsed = (int) (now - lastRequestToUriPrefix[prefix]).TotalSeconds;
            if (elapsed < requestIntervalSeconds)
                await constrainExecutionAsyncDelegate.ConstrainAsync(requestIntervalSeconds - elapsed);

            lastRequestToUriPrefix[prefix] = now;
        }
    }
}