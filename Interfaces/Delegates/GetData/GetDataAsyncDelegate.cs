using System.Threading.Tasks;

namespace Interfaces.Delegates.GetData
{
    public interface IGetDataAsyncDelegate<T>
    {
        Task<T> GetDataAsync(string uri = null);
    }
}