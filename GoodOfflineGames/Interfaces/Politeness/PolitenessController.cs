namespace Interfaces.Politeness
{
    public interface IThrottleDelegate
    {
        void Throttle();
    }

    public interface IPolitenessController:
        IThrottleDelegate
    {
        // ...
    }
}
