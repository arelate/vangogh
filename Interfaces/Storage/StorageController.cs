using System.Threading.Tasks;

namespace Interfaces.Storage
{
    public interface IPullAsyncDelegate<Type>
    {
        // TODO: All Async delegates should take IStatus
        Task<Type> PullAsync(string uri);
    }

    public interface IPushAsyncDelegate<Type>
    {
        // TODO: All Async delegates should take IStatus
        Task PushAsync(string uri, Type data);
    }

    public interface IStorageController<Type> :
        IPullAsyncDelegate<Type>,
        IPushAsyncDelegate<Type>
    {
        // ...
    }
}
