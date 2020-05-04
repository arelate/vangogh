using System.IO;
using System.Threading.Tasks;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using ProtoBuf;

namespace Delegates.Data.Storage
{
    public abstract class PostProtoBufDataAsyncDelegate<T> : IPostDataAsyncDelegate<T>
    {
        private readonly IConvertDelegate<string, Stream> convertUriToWritableStreamDelegate;

        public PostProtoBufDataAsyncDelegate(
            IConvertDelegate<string, Stream> convertUriToWritableStreamDelegate)
        {
            this.convertUriToWritableStreamDelegate = convertUriToWritableStreamDelegate;
        }

        public async Task<string> PostDataAsync(T data, string uri = null)
        {
            await Task.Run(() =>
            {
                using (var writableStream = convertUriToWritableStreamDelegate.Convert(uri))
                {
                    Serializer.Serialize<T>(writableStream, data);
                }
            });

            return uri;
        }
    }
}