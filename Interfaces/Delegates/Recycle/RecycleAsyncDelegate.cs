using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Recycle
{
    public interface IRecycleAsyncDelegate
    {
        Task RecycleAsync(string uri, IStatus status);
    }
}