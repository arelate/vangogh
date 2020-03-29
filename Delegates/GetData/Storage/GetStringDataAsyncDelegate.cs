using System.Threading.Tasks;
using System.IO;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.Convert;


namespace Delegates.GetData.Storage
{
    public sealed class GetStringDataAsyncDelegate : IGetDataAsyncDelegate<string>
    {
        private readonly IConvertDelegate<string, Stream> convertUriToReadableStream;

        [Dependencies(
            "Delegates.Convert.Streams.ConvertUriToReadableStreamDelegate,Delegates")]
        public GetStringDataAsyncDelegate(
            IConvertDelegate<string, Stream> convertUriToReadableStream)
        {
            this.convertUriToReadableStream = convertUriToReadableStream;
        }

        public async Task<string> GetDataAsync(string uri = null)
        {
            var data = string.Empty;

            if (System.IO.File.Exists(uri))
            {
                using (var stream = convertUriToReadableStream.Convert(uri))
                using (StreamReader reader = new StreamReader(stream))
                    data = await reader.ReadToEndAsync();
            }

            return data;
        }
    }
}