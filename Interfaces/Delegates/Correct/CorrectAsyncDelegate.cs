using System.Threading.Tasks;

namespace Interfaces.Delegates.Correct
{
    public interface ICorrectAsyncDelegate<Type>
    {
        Task<Type> CorrectAsync(Type data);
    }
}
