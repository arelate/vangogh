using System.Threading.Tasks;
using System.IO;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

namespace Delegates.PostData.Storage
{
    public class PostStringDataAsyncDelegate : IPostDataAsyncDelegate<string>
    {
        private readonly IConvertDelegate<string, Stream> convertUriToWritableStream;

        [Dependencies(
            DependencyContext.Default,
            "Delegates.Convert.IO.ConvertUriToWritableStreamDelegate,Delegates")]
        public PostStringDataAsyncDelegate(
            IConvertDelegate<string, Stream> convertUriToWritableStream)
        {
            this.convertUriToWritableStream = convertUriToWritableStream;
        }

        public async Task PostDataAsync(string data, string uri = null)
        {
            using (var stream = convertUriToWritableStream.Convert(uri))
            using (StreamWriter writer = new StreamWriter(stream))
                await writer.WriteLineAsync(data);
        }
    }
}