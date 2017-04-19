using System.Threading.Tasks;

namespace Interfaces.Storage
{
    public interface IPullAsyncDelegate<Type>
    {
        Task<Type> PullAsync(string uri);
    }

    public interface IPushAsyncDelegate<Type>
    {
        Task PushAsync(string uri, Type data);
    }

    public interface IStorageController<Type>:
        IPullAsyncDelegate<Type>,
        IPushAsyncDelegate<Type>
    {
        // ...
    }
}
