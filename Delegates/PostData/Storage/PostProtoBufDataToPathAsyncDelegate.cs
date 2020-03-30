using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using ProtoBuf;

namespace Delegates.PostData.Storage
{
    public abstract class PostProtoBufDataToPathAsyncDelegate<T> : IPostDataAsyncDelegate<T>
    {
        private readonly IPostDataAsyncDelegate<T> postProtoBufDataAsyncDelegate;
        private readonly IGetPathDelegate getPathDelegate;

        public PostProtoBufDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<T> postProtoBufDataAsyncDelegate,
            IGetPathDelegate getPathDelegate)
        {
            this.postProtoBufDataAsyncDelegate = postProtoBufDataAsyncDelegate;
            this.getPathDelegate = getPathDelegate;
        }

        public async Task<string> PostDataAsync(T data, string uri = null)
        {
            return await postProtoBufDataAsyncDelegate.PostDataAsync(
                data, 
                getPathDelegate.GetPath(string.Empty, uri));
        }
    }
}