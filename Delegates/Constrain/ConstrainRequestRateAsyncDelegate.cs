using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Constrain;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Find;

using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Delegates.Constrain
{
    public class ConstrainRequestRateAsyncDelegate : IConstrainAsyncDelegate<string>
    {
        readonly IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate;
        readonly IFindDelegate<string> findStringDelegate;
        readonly IActionLogController actionLogController;
        readonly Dictionary<string, DateTime> lastRequestToUriPrefix;
        readonly IItemizeAllDelegate<string> itemizeRateContraindesUris;
        const int requestIntervalSeconds = 30;
        const int passthroughCount = 100; // don't throttle first N requests
        int rateLimitRequestsCount;

        [Dependencies(
            DependencyContext.Default,
            "Delegates.Constrain.ConstrainExecutionAsyncDelegate,Delegates",
            "Delegates.Find.System.FindStringDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers",
            "GOG.Delegates.Itemize.ItemizeAllRateConstrainedUrisDelegate,GOG.Delegates")]
        public ConstrainRequestRateAsyncDelegate(
            IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate,
            IFindDelegate<string> findStringDelegate,
            IActionLogController actionLogController,
            IItemizeAllDelegate<string> itemizeRateContraindesUris)
        {
            this.constrainExecutionAsyncDelegate = constrainExecutionAsyncDelegate;
            this.findStringDelegate = findStringDelegate;
            this.actionLogController = actionLogController;
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
            {
                actionLogController.StartAction("Limit request rate to avoid temporary server block");
                await constrainExecutionAsyncDelegate.ConstrainAsync(requestIntervalSeconds - elapsed);
                actionLogController.CompleteAction();
            }

            lastRequestToUriPrefix[prefix] = now;
        }
    }
}
