using System.IO;
using System.Threading.Tasks;
using Attributes;
using Delegates.Conversions.Streams;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage
{
    public sealed class GetStringDataAsyncDelegate : IGetDataAsyncDelegate<string, string>
    {
        private readonly IConvertDelegate<string, Stream> convertUriToReadableStream;

        [Dependencies(
            typeof(ConvertUriToReadableStreamDelegate))]
        public GetStringDataAsyncDelegate(
            IConvertDelegate<string, Stream> convertUriToReadableStream)
        {
            this.convertUriToReadableStream = convertUriToReadableStream;
        }

        public async Task<string> GetDataAsync(string uri = null)
        {
            var data = string.Empty;

            if (File.Exists(uri))
                using (var stream = convertUriToReadableStream.Convert(uri))
                using (var reader = new StreamReader(stream))
                {
                    data = await reader.ReadToEndAsync();
                }

            return data;
        }
    }
}