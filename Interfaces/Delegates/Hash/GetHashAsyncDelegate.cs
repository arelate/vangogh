using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Hash
{
    public interface IGetHashAsyncDelegate<Type>
    {
        Task<string> GetHashAsync(Type data, IStatus status);
    }
}
