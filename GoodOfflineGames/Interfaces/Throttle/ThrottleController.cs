using Interfaces.TaskStatus;

namespace Interfaces.Throttle
{
    public interface IThrottleDelegate
    {
        void Throttle(ITaskStatus taskStatus);
    }

    public interface IThresholdProperty
    {
        long Threshold { get; }
    }

    public interface IThrottleController:
        IThrottleDelegate,
        IThresholdProperty
    {
        // ...
    }
}
