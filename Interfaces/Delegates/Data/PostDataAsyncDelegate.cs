using System.Threading.Tasks;

namespace Interfaces.Delegates.Data
{
    public interface IPostDataAsyncDelegate<in T>
    {
        Task<string> PostDataAsync(T data, string uri = null);
    }
}