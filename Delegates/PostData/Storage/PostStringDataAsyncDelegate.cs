using System.Threading.Tasks;
using System.IO;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.Convert;


namespace Delegates.PostData.Storage
{
    public class PostStringDataAsyncDelegate : IPostDataAsyncDelegate<string>
    {
        private readonly IConvertDelegate<string, Stream> convertUriToWritableStream;

        [Dependencies(
            "Delegates.Convert.Streams.ConvertUriToWritableStreamDelegate,Delegates")]
        public PostStringDataAsyncDelegate(
            IConvertDelegate<string, Stream> convertUriToWritableStream)
        {
            this.convertUriToWritableStream = convertUriToWritableStream;
        }

        public async Task<string> PostDataAsync(string data, string uri = null)
        {
            using (var stream = convertUriToWritableStream.Convert(uri))
            using (StreamWriter writer = new StreamWriter(stream))
                await writer.WriteLineAsync(data);

            return uri;
        }
    }
}