using System.Threading.Tasks;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

namespace Delegates.PostData.Storage
{
    public abstract class PostJSONDataToPathAsyncDelegate<T> : IPostDataAsyncDelegate<T>
    {
        private readonly IPostDataAsyncDelegate<T> postJSONDataAsyncDelegate;
        private readonly IGetPathDelegate getPathDelegate;

        public PostJSONDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<T> postJSONDataAsyncDelegate,
            IGetPathDelegate getPathDelegate)
        {
            this.postJSONDataAsyncDelegate = postJSONDataAsyncDelegate;
            this.getPathDelegate = getPathDelegate;
        }

        public async Task PostDataAsync(T data, string uri = null)
        {
            await postJSONDataAsyncDelegate.PostDataAsync(
                data,
                getPathDelegate.GetPath(string.Empty, uri));
        }
    }
}