using System.Threading.Tasks;

using Interfaces.Controllers.Data;

using Interfaces.Status;

namespace Interfaces.Delegates.Hash
{
    public interface ISetHashAsyncDelegate<Type>
    {
        Task SetHashAsync(Type data, string hash, IStatus status);
    }
}
