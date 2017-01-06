using System.Threading;

using Interfaces.Throttle;

namespace Controllers.Throttle
{
    public class ThrottleController : IThrottleController
    {
        private int delayMilliseconds; // default is 2 minutes
        private long threshold;

        public ThrottleController(
            long threshold,
            int delayMilliseconds = 1000 * 60 * 2)
        {
            this.threshold = threshold;
            this.delayMilliseconds = delayMilliseconds;
        }

        public void Throttle()
        {
            Thread.Sleep(delayMilliseconds);
        }

        public long Threshold
        {
            get { return threshold; }
        }
    }
}
