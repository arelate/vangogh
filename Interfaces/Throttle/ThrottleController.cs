using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Throttle
{
    public interface IThrottleAsyncDelegate
    {
        Task ThrottleAsync(int seconds, IStatus status);
    }

    public interface IThrottleController:
        IThrottleAsyncDelegate
    {
        // ...
    }
}
