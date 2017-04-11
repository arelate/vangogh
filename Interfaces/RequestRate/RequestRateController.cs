using Interfaces.Status;

namespace Interfaces.RequestRate
{
    public interface IEnforceRequestRateDelegate
    {
        void EnforceRequestRate(string uri, IStatus status);
    }

    public interface IRequestRateController:
        IEnforceRequestRateDelegate
    {
        // ...
    }
}
