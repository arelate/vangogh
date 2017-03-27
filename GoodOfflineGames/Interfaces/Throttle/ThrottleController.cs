using Interfaces.TaskStatus;

namespace Interfaces.Throttle
{
    public interface IThrottleDelegate
    {
        void Throttle(ITaskStatus taskStatus);
    }

    public interface IThrottleController:
        IThrottleDelegate
    {
        // ...
    }
}
