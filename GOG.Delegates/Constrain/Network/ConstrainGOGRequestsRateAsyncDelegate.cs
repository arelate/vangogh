using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Constrain;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Itemize;

namespace GOG.Delegates.Constrain.Network
{
    public class ConstrainGOGRequestRateAsyncDelegate : IConstrainAsyncDelegate<string>
    {
        readonly IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate;
        readonly IFindDelegate<string> findStringDelegate;
        readonly Dictionary<string, DateTime> lastRequestToUriPrefix;
        readonly IItemizeAllDelegate<string> itemizeRateContraindesUris;
        const int requestIntervalSeconds = 30;
        const int passthroughCount = 100; // don't throttle first N requests
        int rateLimitRequestsCount;

        [Dependencies(
            "Delegates.Constrain.ConstrainExecutionAsyncDelegate,Delegates",
            "Delegates.Collections.System.FindStringDelegate,Delegates",
            "GOG.Delegates.Itemize.ItemizeAllRateConstrainedUrisDelegate,GOG.Delegates")]
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
            var elapsed = (int)(now - lastRequestToUriPrefix[prefix]).TotalSeconds;
            if (elapsed < requestIntervalSeconds)
                await constrainExecutionAsyncDelegate.ConstrainAsync(requestIntervalSeconds - elapsed);

            lastRequestToUriPrefix[prefix] = now;
        }
    }
}
