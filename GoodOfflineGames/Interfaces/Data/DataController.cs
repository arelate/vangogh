using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Data
{
    public interface ILoadDelegate
    {
        Task LoadAsync();
    }

    public interface ISaveDelegate
    {
        Task SaveAsync();
    }

    public interface IGetByIdDelegate<Type>
    {
        Task<Type> GetByIdAsync(long id);
    }

    public interface IEnumerateIdsDelegate
    {
        IEnumerable<long> EnumerateIds();
    }

    public interface IUpdateDelegate<Type>
    {
        Task UpdateAsync(params Type[] data);
    }

    public interface IRemoveDelegate<Type>
    {
        Task RemoveAsync(params Type[] data);
    }

    public interface IContainsDelegate<Type>
    {
        bool Contains(Type data);
    }

    public interface IContainsIdDelegate
    {
        bool ContainsId(long id);
    }

    public interface ICountDelegate
    {
        int Count();
    }

    public interface IDataController<Type>:
        ILoadDelegate,
        ISaveDelegate,
        IEnumerateIdsDelegate,
        ICountDelegate,
        IGetByIdDelegate<Type>,
        IUpdateDelegate<Type>,
        IRemoveDelegate<Type>,
        IContainsDelegate<Type>,
        IContainsIdDelegate
    {
        // ...
    }
}
