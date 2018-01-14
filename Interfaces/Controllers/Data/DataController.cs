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

    public interface IGetByIdAsyncDelegate<IdentityType, ReturnType>
    {
        Task<ReturnType> GetByIdAsync(IdentityType id, IStatus status);
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

    public interface IContainsIdAsyncDelegate<IdentityType>
    {
        Task<bool> ContainsIdAsync(IdentityType id, IStatus status);
    }

    public interface ICountAsyncDelegate
    {
        Task<int> CountAsync(IStatus status);
    }

    public interface IDataAvailableDelegate
    {
        bool DataAvailable { get; }
    }

    public interface IGetCreatedAsyncDelegate<IdentityType>
    {
        Task<DateTime> GetCreatedAsync(IdentityType id, IStatus status);
    }

    public interface IDataController<DataType> :
        IDataAvailableDelegate,
        ILoadAsyncDelegate,
        ISaveAsyncDelegate,
        IItemizeAllAsyncDelegate<long>,
        ICountAsyncDelegate,
        IGetByIdAsyncDelegate<long, DataType>,
        IUpdateAsyncDelegate<DataType>,
        IRemoveAsyncDelegate<DataType>,
        IContainsAsyncDelegate<DataType>,
        IContainsIdAsyncDelegate<long>,
        IGetCreatedAsyncDelegate<long>,
        IItemizeAsyncDelegate<DateTime, long> // all items modified on or after a certain date
    {
        // ...
    }
}
