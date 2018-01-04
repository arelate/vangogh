using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.RequestRate
{
    public interface IEnforceRequestRateAsyncDelegate
    {
        Task EnforceRequestRateAsync(string uri, IStatus status);
    }

    public interface IRequestRateController:
        IEnforceRequestRateAsyncDelegate
    {
        // ...
    }
}
