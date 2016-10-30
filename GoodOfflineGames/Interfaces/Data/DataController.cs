using System.Threading.Tasks;

namespace Interfaces.Data
{
    public interface ILoadDelegate
    {
        Task Load();
    }

    public interface ISaveDelegate
    {
        Task Save();
    }

    public interface IGetByIdDelegate<Type>
    {
        Task<Type> GetById(long id);
    }

    public interface IUpdateDelegate<Type>
    {
        Task Update(params Type[] data);
    }

    public interface IRemoveDelegate<Type>
    {
        Task Remove(params Type[] data);
    }

    public interface IContainsDelegate<Type>
    {
        bool Contains(Type data);
    }

    public interface IDataController<Type>:
        ILoadDelegate,
        ISaveDelegate,
        IGetByIdDelegate<Type>,
        IUpdateDelegate<Type>,
        IRemoveDelegate<Type>,
        IContainsDelegate<Type>
    {
        // ...
    }
}
