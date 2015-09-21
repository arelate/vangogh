using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface IPullController<Type>
    {
        Task<Type> Pull(string uri);
    }

    public interface IPushController<Type>
    {
        Task Push(string uri, Type data);
    }

    public interface IStorageController<Type>:
        IPullController<Type>,
        IPushController<Type>
    {
        // ...
    }
}
