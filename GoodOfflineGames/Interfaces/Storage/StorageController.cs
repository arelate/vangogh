using System.Threading.Tasks;

namespace Interfaces.Storage
{
    public interface IPullDelegate<Type>
    {
        Task<Type> PullAsync(string uri);
    }

    public interface IPushDelegate<Type>
    {
        Task PushAsync(string uri, Type data);
    }

    public interface IStorageController<Type>:
        IPullDelegate<Type>,
        IPushDelegate<Type>
    {
        // ...
    }
}
