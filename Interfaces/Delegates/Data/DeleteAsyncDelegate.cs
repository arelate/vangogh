using System.Threading.Tasks;

namespace Interfaces.Delegates.Data
{
    public interface IDeleteAsyncDelegate<in Type>
    {
        Task DeleteAsync(Type data);
    }
}