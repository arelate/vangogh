using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Throttle
{
    public interface IThrottleAsyncDelegate
    {
        Task ThrottleAsync(int seconds, IStatus status);
    }
}
