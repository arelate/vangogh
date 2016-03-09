using System.Threading.Tasks;

namespace GOG.Interfaces
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
