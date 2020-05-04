using System.Threading.Tasks;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage
{
    public abstract class PostJSONDataAsyncDelegate<T> : IPostDataAsyncDelegate<T>
    {
        private readonly IPostDataAsyncDelegate<string> postStringDataAsyncDelegate;
        private readonly IConvertDelegate<T, string> convertTypeToJSONDelegate;

        public PostJSONDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate,
            IConvertDelegate<T, string> convertTypeToJSONDelegate)
        {
            this.postStringDataAsyncDelegate = postStringDataAsyncDelegate;
            this.convertTypeToJSONDelegate = convertTypeToJSONDelegate;
        }

        public async Task<string> PostDataAsync(T data, string uri = null)
        {
            var serializedData = convertTypeToJSONDelegate.Convert(data);
            return await postStringDataAsyncDelegate.PostDataAsync(serializedData, uri);
        }
    }
}