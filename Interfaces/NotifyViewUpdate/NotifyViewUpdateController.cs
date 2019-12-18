using System.Threading.Tasks;

namespace Interfaces.NotifyViewUpdate
{
    public interface IGetViewUpdateAsyncDelegate<T>
    {
        Task<T> GetViewUpdateAsync();
    }

    public interface INotifyViewUpdateOutputOnRefreshAsyncDelegate
    {
        Task NotifyViewUpdateOutputOnRefreshAsync();
    }

    public interface INotifyViewUpdateOutputContinuousAsyncDelegate
    {
        Task NotifyViewUpdateOutputContinuousAsync();
    }

    public interface INotifyViewUpdateController:
        INotifyViewUpdateOutputOnRefreshAsyncDelegate,
        INotifyViewUpdateOutputContinuousAsyncDelegate
    {
        // ...
    }
}
