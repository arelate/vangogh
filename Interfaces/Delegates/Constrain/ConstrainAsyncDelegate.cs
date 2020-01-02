using System.Threading.Tasks;

namespace Interfaces.Delegates.Constrain
{
    public interface IConstrainAsyncDelegate<Type>
    {
        Task ConstrainAsync(Type data);
    }
}
