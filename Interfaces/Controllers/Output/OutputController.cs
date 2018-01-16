using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Controllers.Output
{
    public interface ISetRefreshAsyncDelegate
    {
        Task SetRefreshAsync();
    }

    public interface IClearContinuousLinesAsyncDelegate
    {
        Task ClearContinuousLinesAsync(int lines);
    }

    public interface IOutputOnRefreshAsyncDelegate<T>
    {
        Task OutputOnRefreshAsync(T data);
    }

    public interface IOutputFixedOnRefreshAsyncDelegate<T>
    {
        Task OutputFixedOnRefreshAsync(T data);
    }

    public interface IOutputContinuousAsyncDelegate<T>
    {
        Task OutputContinuousAsync(IStatus status, T data);
    }

    public interface IOutputController<T> :
        ISetRefreshAsyncDelegate,
        IClearContinuousLinesAsyncDelegate,
        IOutputOnRefreshAsyncDelegate<T>,
        IOutputFixedOnRefreshAsyncDelegate<T>,
        IOutputContinuousAsyncDelegate<T>
    {
        // ...
    }
}
