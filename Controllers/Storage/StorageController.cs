using System;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Delegates.Convert;
using Interfaces.Controllers.Storage;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Controllers.Storage
{
    public class StorageController : IStorageController<string>
    {
        readonly IConvertDelegate<string, System.IO.Stream> convertUriToReadableStream;

        [Dependencies(
            DependencyContext.Default,
            "Delegates.Convert.IO.ConvertUriToReadableStreamDelegate,Delegates")]
        public StorageController(
            IConvertDelegate<string, System.IO.Stream> convertUriToReadableStream)
        {
            this.convertUriToReadableStream = convertUriToReadableStream;
        }

        public async Task PushAsync(
            string uri,
            string data)
        {
            using (var stream = convertUriToReadableStream.Convert(uri))
            using (StreamWriter writer = new StreamWriter(stream))
                await writer.WriteLineAsync(data);
        }

        public async Task<string> PullAsync(string uri)
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
