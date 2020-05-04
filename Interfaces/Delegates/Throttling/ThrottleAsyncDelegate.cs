using System.Threading.Tasks;

namespace Interfaces.Delegates.Throttling
{
    public interface IThrottleAsyncDelegate<Type>
    {
        Task ThrottleAsync(Type data);
    }
}