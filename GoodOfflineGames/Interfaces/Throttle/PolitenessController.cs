namespace Interfaces.Throttle
{
    public interface IThrottleDelegate
    {
        void Throttle();
    }

    public interface IThrottleController:
        IThrottleDelegate
    {
        // ...
    }
}
