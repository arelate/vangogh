using System.Threading;

using Interfaces.Throttle;

namespace Controllers.Throttle
{
    public class ThrottleController : IThrottleController
    {
        private int delayMilliseconds; // default is 2 minutes

        public ThrottleController(int delayMilliseconds = 1000 * 60 * 2) 
        {
            this.delayMilliseconds = delayMilliseconds;
        }

        public void Throttle()
        {
            Thread.Sleep(delayMilliseconds);
        }
    }
}
