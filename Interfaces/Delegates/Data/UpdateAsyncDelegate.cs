using System.Threading.Tasks;

namespace Interfaces.Delegates.Data
{
    public interface IUpdateAsyncDelegate<in Type>
    {
        Task UpdateAsync(Type data);
    }
}