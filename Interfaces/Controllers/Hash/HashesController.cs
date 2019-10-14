using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Itemize;

using Interfaces.Status;

using Interfaces.Controllers.Data;

namespace Interfaces.Controllers.Hashes
{
    public interface ISetHashAsyncDelegate<Type>
    {
        Task SetHashAsync(Type data, string hash, IStatus status);
    }

    public interface IHashesController:
        IConvertAsyncDelegate<string, Task<string>>,
        ISetHashAsyncDelegate<string>,
        IItemizeAllAsyncDelegate<string>,
        ICommitAsyncDelegate
    {
        // ...
    }
}
