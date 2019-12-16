using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Constrain;
using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Collection;

using Interfaces.Status;

using Attributes;

namespace Delegates.Constrain
{
    public class ConstrainRequestRateAsyncDelegate : IConstrainAsyncDelegate<string>
    {
        readonly IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate;
        readonly ICollectionController collectionController;
        readonly IStatusController statusController;
        readonly Dictionary<string, DateTime> lastRequestToUriPrefix;
        readonly IItemizeAllDelegate<string> itemizeRateContraindesUris;
        const int requestIntervalSeconds = 30;
        const int passthroughCount = 100; // don't throttle first N requests
        int rateLimitRequestsCount;

        [Dependencies(
            "Delegates.Constrain.ConstrainExecutionAsyncDelegate,Delegates",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers",
            "GOG.Delegates.Itemize.ItemizeAllRateConstrainedUrisDelegate,GOG.Delegates")]
        public ConstrainRequestRateAsyncDelegate(
            IConstrainAsyncDelegate<int> constrainExecutionAsyncDelegate,
            ICollectionController collectionController,
            IStatusController statusController,
            IItemizeAllDelegate<string> itemizeRateContraindesUris)
        {
            this.constrainExecutionAsyncDelegate = constrainExecutionAsyncDelegate;
            this.collectionController = collectionController;
            this.statusController = statusController;
            lastRequestToUriPrefix = new Dictionary<string, DateTime>();
            rateLimitRequestsCount = 0;

            this.itemizeRateContraindesUris = itemizeRateContraindesUris;

            if (this.itemizeRateContraindesUris != null)
                foreach (var uri in this.itemizeRateContraindesUris.ItemizeAll())
                    lastRequestToUriPrefix.Add(
                        uri,
                        DateTime.UtcNow - TimeSpan.FromSeconds(requestIntervalSeconds));
        }

        public async Task ConstrainAsync(string uri, IStatus status)
        {
            var prefix = collectionController.Reduce(itemizeRateContraindesUris.ItemizeAll(), uri.StartsWith).SingleOrDefault();
            if (string.IsNullOrEmpty(prefix)) return;

            // don't limit rate for the first N requests, even if they match rate limit prefix
            if (++rateLimitRequestsCount <= passthroughCount) return;

            var now = DateTime.UtcNow;
            var elapsed = (int)(now - lastRequestToUriPrefix[prefix]).TotalSeconds;
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
