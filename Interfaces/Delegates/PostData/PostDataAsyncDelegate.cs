using System.Threading.Tasks;

namespace Interfaces.Delegates.PostData
{
    public interface IPostDataAsyncDelegate<T>
    {
        Task PostDataAsync(T data, string uri = null);
    }
}