using System.Threading.Tasks;

namespace Interfaces.Data
{
    public interface IInitializeDelegate
    {
        Task Initialize();
    }

    public interface IGetByIdDelegate<Type>
    {
        Task<Type> GetById(long id);
    }

    public interface IUpdateDelegate<Type>
    {
        Task Update(Type product);
    }

    public interface IRemoveDelegate<Type>
    {
        Task Remove(Type product);
    }

    public interface IContainsDelegate<Type>
    {
        bool Contains(Type product);
    }

    public interface IDataController<Type>:
        IInitializeDelegate,
        IGetByIdDelegate<Type>,
        IUpdateDelegate<Type>,
        IRemoveDelegate<Type>,
        IContainsDelegate<Type>
    {
        // ...
    }
}
