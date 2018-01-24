using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Status;

namespace Interfaces.Controllers.Data
{
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
        Task UpdateAsync(Type data, IStatus status);
    }

    public interface ICreateAsyncDelegate<Type>
    {
        Task CreateAsync(Type data, IStatus status);
    }

    public interface IDeleteAsyncDelegate<Type>
    {
        Task DeleteAsync(Type data, IStatus status);
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

    public interface IDataController<IdentityType, DataType> :
        IItemizeAllAsyncDelegate<IdentityType>,
        ICountAsyncDelegate,
        IGetByIdAsyncDelegate<IdentityType, DataType>,
        IUpdateAsyncDelegate<DataType>,
        IDeleteAsyncDelegate<DataType>,
        IContainsAsyncDelegate<DataType>,
        IContainsIdAsyncDelegate<IdentityType>
    {
        // ...
    }
}
