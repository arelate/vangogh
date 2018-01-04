using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.ViewUpdates
{
    public interface IGetViewUpdateAsyncDelegate<T>
    {
        Task<T> GetViewUpdateAsync();
    }

    public interface IPostViewUpdateDelegate
    {
        void PostViewUpdate();
    }

    public interface IPostViewUpdateAsyncDelegate
    {
        Task PostViewUpdateAsync();
    }
}
