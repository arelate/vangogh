using System.Threading.Tasks;

namespace Interfaces.IO.Storage
{
    public interface IPullDelegate<Type>
    {
        Task<Type> Pull(string uri);
    }

    public interface IPushDelegate<Type>
    {
        Task Push(string uri, Type data);
    }

    public interface IStorageController<Type>:
        IPullDelegate<Type>,
        IPushDelegate<Type>
    {
        // ...
    }
}
