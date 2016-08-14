using System.Threading;

using Interfaces.Politeness;

namespace Controllers.Politeness
{
    public class PolitenessController : IPolitenessController
    {
        private int delayMilliseconds; // default is 2 minutes

        public PolitenessController(int delayMilliseconds = 1000 * 60 * 2) 
        {
            this.delayMilliseconds = delayMilliseconds;
        }

        public void Throttle()
        {
            Thread.Sleep(delayMilliseconds);
        }
    }
}
