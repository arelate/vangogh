using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage
{
    public abstract class PostJSONDataToPathAsyncDelegate<T> : IPostDataAsyncDelegate<T>
    {
        private readonly IPostDataAsyncDelegate<T> postJSONDataAsyncDelegate;
        private readonly IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate;

        public PostJSONDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<T> postJSONDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate)
        {
            this.postJSONDataAsyncDelegate = postJSONDataAsyncDelegate;
            this.getPathDelegate = getPathDelegate;
        }

        public async Task<string> PostDataAsync(T data, string uri = null)
        {
            return await postJSONDataAsyncDelegate.PostDataAsync(
                data,
                getPathDelegate.GetValue((string.Empty, uri)));
        }
    }
}