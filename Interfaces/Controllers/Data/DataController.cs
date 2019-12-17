using System.Threading.Tasks;

using Interfaces.Delegates.Itemize;

using Interfaces.Status;

namespace Interfaces.Controllers.Data
{
    public interface IGetByIdAsyncDelegate<IdentityType, DataType>
    {
        Task<DataType> GetByIdAsync(IdentityType id, IStatus status);
    }

    public interface IUpdateAsyncDelegate<Type>
    {
        Task UpdateAsync(Type data, IStatus status);
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

    public interface ICommitAsyncDelegate
    {
        Task CommitAsync(IStatus status);
    }

    public interface IDataController<DataType> :
        IItemizeAllAsyncDelegate<DataType>,
        ICountAsyncDelegate,
        IGetByIdAsyncDelegate<long, DataType>,
        IUpdateAsyncDelegate<DataType>,
        IDeleteAsyncDelegate<DataType>,
        IContainsAsyncDelegate<DataType>,
        IContainsIdAsyncDelegate<long>,
        ICommitAsyncDelegate
    {
        // ...
    }
}
