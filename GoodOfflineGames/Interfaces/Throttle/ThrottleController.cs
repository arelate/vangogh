namespace Interfaces.Throttle
{
    public interface IThrottleDelegate
    {
        void Throttle();
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
