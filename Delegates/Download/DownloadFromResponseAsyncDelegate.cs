using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Interfaces.Controllers.Network;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Download;
using Interfaces.Delegates.Convert;


using Attributes;

using Models.Units;

namespace Delegates.Download
{
    public class DownloadFromResponseAsyncDelegate : IDownloadFromResponseAsyncDelegate
    {
        readonly IConvertDelegate<string, System.IO.Stream> convertUriToWritableRStream;
        readonly IActionLogController actionLogController;

        [Dependencies(
            "Delegates.Convert.Streams.ConvertUriToWritableDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public DownloadFromResponseAsyncDelegate(
            IConvertDelegate<string, System.IO.Stream> convertUriToWritableRStream,
            IActionLogController actionLogController)
        {
            this.convertUriToWritableRStream = convertUriToWritableRStream;
            this.actionLogController = actionLogController;
        }

        public async Task DownloadFromResponseAsync(HttpResponseMessage response, string destination)
        {
            response.EnsureSuccessStatusCode();

            var filename = response.RequestMessage.RequestUri.Segments.Last();
            var fullPath = Path.Combine(destination, filename);

            int bufferSize = 1024 * 1024; // 1M
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long totalBytesRead = 0;

            // don't redownload file with the same name and size
            if (File.Exists(fullPath) &&
                new FileInfo(fullPath).Length == response.Content.Headers.ContentLength)
            {
                // await statusController.InformAsync(
                //     status, 
                //     $"File {fullPath} already exists and matches response size, will not be redownloading");
                return;
            }

            using (var writeableStream = convertUriToWritableRStream.Convert(fullPath))
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                {
                    totalBytesRead += bytesRead;
                    var contentLength = response.Content.Headers.ContentLength != null ?
                        (long) response.Content.Headers.ContentLength :
                        totalBytesRead;
                    await writeableStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
        }
    }
}
