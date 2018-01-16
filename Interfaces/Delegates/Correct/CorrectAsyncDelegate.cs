using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.Correct
{
    public interface ICorrectAsyncDelegate<Type>
    {
        Task<Type> CorrectAsync(Type data, IStatus status);
    }
}
