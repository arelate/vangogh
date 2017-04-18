using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Data
{
    public interface ILoadAsyncDelegate
    {
        Task LoadAsync();
    }

    public interface ISaveAsyncDelegate
    {
        Task SaveAsync();
    }

    public interface IGetByIdAsyncDelegate<Type>
    {
        Task<Type> GetByIdAsync(long id);
    }

    public interface IEnumerateIdsDelegate
    {
        IEnumerable<long> EnumerateIds();
    }

    public interface IUpdateAsyncDelegate<Type>
    {
        Task UpdateAsync(IStatus status, params Type[] data);
    }
    public interface IRemoveAsyncDelegate<Type>
    {
        Task RemoveAsync(IStatus status, params Type[] data);
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
        ILoadAsyncDelegate,
        ISaveAsyncDelegate,
        IEnumerateIdsDelegate,
        ICountDelegate,
        IGetByIdAsyncDelegate<Type>,
        IUpdateAsyncDelegate<Type>,
        IRemoveAsyncDelegate<Type>,
        IContainsDelegate<Type>,
        IContainsIdDelegate
    {
        // ...
    }
}
