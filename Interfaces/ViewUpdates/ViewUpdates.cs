using System.Threading.Tasks;

namespace Interfaces.ViewUpdates
{
    public interface IGetViewUpdateDelegate<T>
    {
        T GetViewUpdate();
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
