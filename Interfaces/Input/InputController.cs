using System.Threading.Tasks;

namespace Interfaces.Input
{
    public interface IRequestInputAsyncDelegate<T>
    {
        Task<T> RequestInputAsync(string message);
    }

    public interface IRequestPrivateInputAsyncDelegate<T>
    {
        Task<T> RequestPrivateInputAsync(string message);
    }

    public interface IInputController<T>:
        IRequestInputAsyncDelegate<T>,
        IRequestPrivateInputAsyncDelegate<T>
    {
        // ...
    }
}
