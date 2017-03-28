using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.RequestRate;
using Interfaces.Throttle;
using Interfaces.TaskStatus;
using Interfaces.Collection;

namespace Controllers.RequestRate
{
    public class RequestRateController : IRequestRateController
    {
        private IThrottleController throttleController;
        private ICollectionController collectionController;
        private ITaskStatusController taskStatusController;
        private Dictionary<string, DateTime> lastRequestToUriPrefix;
        private string[] uriPrefixes;
        private const int requestIntervalSeconds = 2 * 60;

        public RequestRateController(
            IThrottleController throttleController,
            ICollectionController collectionController,
            ITaskStatusController taskStatusController,
            params string[] uriPrefixes)
        {
            this.throttleController = throttleController;
            this.collectionController = collectionController;
            this.taskStatusController = taskStatusController;
            lastRequestToUriPrefix = new Dictionary<string, DateTime>();

            this.uriPrefixes = uriPrefixes;

            if (this.uriPrefixes != null)
                foreach (var prefix in this.uriPrefixes)
                    lastRequestToUriPrefix.Add(
                        prefix, 
                        DateTime.UtcNow - TimeSpan.FromSeconds(requestIntervalSeconds));
        }

        public void EnforceRequestRate(string uri, ITaskStatus taskStatus)
        {
            var prefix = collectionController.Reduce(uriPrefixes, p => uri.StartsWith(p)).SingleOrDefault();
            if (string.IsNullOrEmpty(prefix)) return;

            var now = DateTime.UtcNow;
            var elapsed = (int) (now - lastRequestToUriPrefix[prefix]).TotalSeconds;
            if (elapsed < requestIntervalSeconds)
            {
                var limitRateTask = taskStatusController.Create(taskStatus, "Limit request rate to avoid temporary server block");
                throttleController.Throttle(requestIntervalSeconds - elapsed, taskStatus);
                taskStatusController.Complete(limitRateTask);
            }

            lastRequestToUriPrefix[prefix] = now;
        }
    }
}
