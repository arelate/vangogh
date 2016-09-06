using System.Linq;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Download;
using Interfaces.Network;
using Interfaces.Stream;
using Interfaces.Reporting;

namespace Controllers.Download
{
    public class DownloadController : IDownloadController
    {
        private INetworkController networkController;
        private IStreamController streamController;
        private IDownloadReportingController downloadReportingController;

        public DownloadController(
            INetworkController networkController,
            IStreamController streamController,
            IDownloadReportingController downloadReportingController)
        {
            this.networkController = networkController;
            this.streamController = streamController;

            this.downloadReportingController = downloadReportingController;
        }

        public async Task DownloadFile(string uri, string destination)
        {
            using (var response = await networkController.GetResponse(uri))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength;
                if (totalBytes == null) totalBytes = 0;

                var requestUri = response.RequestMessage.RequestUri;
                var filename = requestUri.Segments.Last();
                var fullPath = Path.Combine(destination, filename);

                int bufferSize = 1024 * 1024; // 1M
                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;
                long totalBytesRead = 0;

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
        }
    }
}
