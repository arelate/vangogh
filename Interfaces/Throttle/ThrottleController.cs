using Interfaces.Status;

namespace Interfaces.Throttle
{
    public interface IThrottleDelegate
    {
        void Throttle(int seconds, IStatus status);
    }

    public interface IThrottleController:
        IThrottleDelegate
    {
        // ...
    }
}
