using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.NotifyViewUpdate
{
    public interface IGetViewUpdateAsyncDelegate<T>
    {
        Task<T> GetViewUpdateAsync(IStatus status);
    }

    public interface INotifyViewUpdateOutputOnRefreshAsyncDelegate
    {
        Task NotifyViewUpdateOutputOnRefreshAsync(IStatus status);
    }

    public interface INotifyViewUpdateOutputContinuousAsyncDelegate
    {
        Task NotifyViewUpdateOutputContinuousAsync(IStatus status);
    }

    public interface INotifyViewUpdateController:
        INotifyViewUpdateOutputOnRefreshAsyncDelegate,
        INotifyViewUpdateOutputContinuousAsyncDelegate
    {
        // ...
    }
}
