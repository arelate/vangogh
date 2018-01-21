using System.Threading.Tasks;

using Interfaces.Delegates.Hash;
using Interfaces.Delegates.Itemize;

using Interfaces.Status;

namespace Interfaces.Controllers.Hash
{
    public interface IStoredHashController:
        IGetHashAsyncDelegate<string>,
        ISetHashAsyncDelegate<string>,
        IItemizeAllAsyncDelegate<string>
    {
        // ...
    }
}
