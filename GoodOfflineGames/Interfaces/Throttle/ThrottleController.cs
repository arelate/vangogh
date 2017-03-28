using Interfaces.TaskStatus;

namespace Interfaces.Throttle
{
    public interface IThrottleDelegate
    {
        void Throttle(int seconds, ITaskStatus taskStatus);
    }

    public interface IThrottleController:
        IThrottleDelegate
    {
        // ...
    }
}
