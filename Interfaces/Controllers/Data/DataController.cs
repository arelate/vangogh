using System.Threading.Tasks;
using Interfaces.Delegates.Itemize;

namespace Interfaces.Controllers.Data
{
    public interface IGetByIdAsyncDelegate<IdentityType, DataType>
    {
        Task<DataType> GetByIdAsync(IdentityType id);
    }

    public interface IUpdateAsyncDelegate<Type>
    {
        Task UpdateAsync(Type data);
    }

    public interface IDeleteAsyncDelegate<Type>
    {
        Task DeleteAsync(Type data);
    }

    public interface IContainsAsyncDelegate<Type>
    {
        Task<bool> ContainsAsync(Type data);
    }

    public interface IContainsIdAsyncDelegate<IdentityType>
    {
        Task<bool> ContainsIdAsync(IdentityType id);
    }

    public interface ICountAsyncDelegate
    {
        Task<int> CountAsync();
    }

    public interface ICommitAsyncDelegate
    {
        Task CommitAsync();
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