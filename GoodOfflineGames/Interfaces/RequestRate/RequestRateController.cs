using Interfaces.TaskStatus;

namespace Interfaces.RequestRate
{
    public interface IEnforceRequestRateDelegate
    {
        void EnforceRequestRate(string uri, ITaskStatus taskStatus);
    }

    public interface IRequestRateController:
        IEnforceRequestRateDelegate
    {
        // ...
    }
}
