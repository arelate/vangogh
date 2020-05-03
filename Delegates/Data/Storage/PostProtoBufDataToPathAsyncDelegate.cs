using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage
{
    public abstract class PostProtoBufDataToPathAsyncDelegate<T> : IPostDataAsyncDelegate<T>
    {
        private readonly IPostDataAsyncDelegate<T> postProtoBufDataAsyncDelegate;
        private readonly IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate;

        public PostProtoBufDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<T> postProtoBufDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate)
        {
            this.postProtoBufDataAsyncDelegate = postProtoBufDataAsyncDelegate;
            this.getPathDelegate = getPathDelegate;
        }

        public async Task<string> PostDataAsync(T data, string uri = null)
        {
            return await postProtoBufDataAsyncDelegate.PostDataAsync(
                data,
                getPathDelegate.GetValue((string.Empty, uri)));
        }
    }
}