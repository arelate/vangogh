using System.Threading.Tasks;

namespace Interfaces.Delegates.Data
{
    public interface IGetDataAsyncDelegate<T>
    {
        Task<T> GetDataAsync(string uri = null);
    }
}