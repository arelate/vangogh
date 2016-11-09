using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Interfaces.Download;
using Interfaces.Network;
using Interfaces.Stream;
using Interfaces.Reporting;
using Interfaces.File;

namespace Controllers.Download
{
    public class DownloadController : IDownloadController
    {
        private INetworkController networkController;
        private IStreamController streamController;
        private IDownloadReportingController downloadReportingController;
        private IFileController fileController;

        public DownloadController(
            INetworkController networkController,
            IStreamController streamController,
            IFileController fileController,
            IDownloadReportingController downloadReportingController)
        {
            this.networkController = networkController;
            this.streamController = streamController;
            this.fileController = fileController;

            this.downloadReportingController = downloadReportingController;
        }

        public async Task<string> DownloadFile(string uri, string destination)
        {
            var responseUriString = string.Empty;

            using (var response = await networkController.GetResponse(HttpMethod.Get, uri))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength;
                if (totalBytes == null) totalBytes = 0;

                var responseUri = response.RequestMessage.RequestUri;
                responseUriString = responseUri.ToString();

                var filename = responseUri.Segments.Last();
                var fullPath = Path.Combine(destination, filename);

                int bufferSize = 1024 * 1024; // 1M
                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;
                long totalBytesRead = 0;

                // don't redownload file with the same name and size
                if (fileController.Exists(fullPath) &&
                    fileController.GetSize(fullPath) == totalBytes)
                    return responseUriString;

                downloadReportingController?.StartTask(string.Empty);

                using (var writeableStream = streamController.OpenWritable(fullPath)) 
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        totalBytesRead += bytesRead;
                        await writeableStream.WriteAsync(buffer, 0, bytesRead);
                        downloadReportingController?.ReportProgress(totalBytesRead, (long)totalBytes);
                    }
                }

                downloadReportingController?.CompleteTask();
            }

            return responseUriString;
        }
    }
}
