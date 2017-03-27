using System;

using Interfaces.RequestRate;
using Interfaces.Throttle;
using Interfaces.TaskStatus;

namespace Controllers.RequestRate
{
    public class RequestRateController : IRequestRateController
    {
        private IThrottleController throttleController;

        public RequestRateController(
            IThrottleController throttleController,
            params string[] ratedUriPrefixes)
        {
            this.throttleController = throttleController;
        }

        public void EnforceRequestRate(string uri, ITaskStatus taskStatus)
        {
            throw new NotImplementedException();
        }
    }
}
