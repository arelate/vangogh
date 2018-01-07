using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Constrain
{
    public interface IConstrainAsyncDelegate<Type>
    {
        Task ConstrainAsync(Type data, IStatus status);
    }
}
