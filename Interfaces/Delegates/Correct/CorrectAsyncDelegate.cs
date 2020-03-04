using System.Threading.Tasks;

namespace Interfaces.Delegates.Correct
{
    // TODO: Request delegate?
    public interface ICorrectAsyncDelegate<Type>
    {
        Task<Type> CorrectAsync(Type data);
    }
}
