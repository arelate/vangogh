using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Status;

namespace Interfaces.Controllers.Data
{
    public interface ILoadAsyncDelegate
    {
        Task LoadAsync(IStatus status);
    }

    public interface ISaveAsyncDelegate
    {
        Task SaveAsync(IStatus status);
    }

    public interface IGetByIdAsyncDelegate<Type>
    {
        Task<Type> GetByIdAsync(long id, IStatus status);
    }

    public interface IEnumerateKeysAsyncDelegate<T>
    {
        Task<IEnumerable<T>> EnumerateKeysAsync(IStatus status);
    }

    public interface IUpdateAsyncDelegate<Type>
    {
        Task UpdateAsync(IStatus status, params Type[] data);
    }
    public interface IRemoveAsyncDelegate<Type>
    {
        Task RemoveAsync(IStatus status, params Type[] data);
    }

    public interface IContainsAsyncDelegate<Type>
    {
        Task<bool> ContainsAsync(Type data, IStatus status);
    }

    public interface IContainsIdAsyncDelegate
    {
        Task<bool> ContainsIdAsync(long id, IStatus status);
    }

    public interface ICountAsyncDelegate
    {
        Task<int> CountAsync(IStatus status);
    }

    public interface IDataAvailableDelegate
    {
        bool DataAvailable { get; }
    }

    public interface IGetLastModifiedAsyncDelegate
    {
        Task<DateTime> GetLastModifiedAsync(long id, IStatus status);
    }

    public interface IDataController<Type>:
        IDataAvailableDelegate,
        ILoadAsyncDelegate,
        ISaveAsyncDelegate,
        IItemizeAllAsyncDelegate<long>,
        ICountAsyncDelegate,
        IGetByIdAsyncDelegate<Type>,
        IUpdateAsyncDelegate<Type>,
        IRemoveAsyncDelegate<Type>,
        IContainsAsyncDelegate<Type>,
        IContainsIdAsyncDelegate,
        IGetLastModifiedAsyncDelegate,
        IItemizeAsyncDelegate<DateTime, long> // all items modified on or after a certain date
    {
        // ...
    }
}
