using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.RequestRate;
using Interfaces.Throttle;
using Interfaces.TaskStatus;

namespace Controllers.RequestRate
{
    public class RequestRateController : IRequestRateController
    {
        private IThrottleController throttleController;
        private Dictionary<string, DateTime> lastRequestToUriPrefix;
        private string[] uriPrefixes;

        public RequestRateController(
            IThrottleController throttleController,
            params string[] uriPrefixes)
        {
            this.throttleController = throttleController;
            lastRequestToUriPrefix = new Dictionary<string, DateTime>();

            this.uriPrefixes = uriPrefixes;

            if (this.uriPrefixes != null)
                foreach (var prefix in this.uriPrefixes)
                    lastRequestToUriPrefix.Add(prefix, DateTime.MinValue);
        }



        public void EnforceRequestRate(string uri, ITaskStatus taskStatus)
        {
            //var prefix = uriPrefixes.

            throw new NotImplementedException();
        }
    }
}
