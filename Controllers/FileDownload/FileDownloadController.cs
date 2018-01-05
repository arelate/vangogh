using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using System;

using Interfaces.FileDownload;
using Interfaces.Network;
using Interfaces.Stream;
using Interfaces.Status;
using Interfaces.File;

using Models.Units;

namespace Controllers.FileDownload
{
    public class FileDownloadController : IFileDownloadController
    {
        private INetworkController networkController;
        private IStreamController streamController;
        private IFileController fileController;
        private IStatusController statusController;

        public FileDownloadController(
            INetworkController networkController,
            IStreamController streamController,
            IFileController fileController,
            IStatusController statusController)
        {
            this.networkController = networkController;
            this.streamController = streamController;
            this.fileController = fileController;

            this.statusController = statusController;
        }

        public async Task DownloadFileFromResponseAsync(HttpResponseMessage response, string destination, IStatus status)
        {
            response.EnsureSuccessStatusCode();

            var filename = response.RequestMessage.RequestUri.Segments.Last();
            var fullPath = Path.Combine(destination, filename);

            int bufferSize = 1024 * 1024; // 1M
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long totalBytesRead = 0;

            // don't redownload file with the same name and size
            if (fileController.Exists(fullPath) &&
                fileController.GetSize(fullPath) == response.Content.Headers.ContentLength)
            {
                await statusController.InformAsync(
                    status, 
                    $"File {fullPath} already exists and matches response size, will not be redownloading");
                return;
            }

            using (var writeableStream = streamController.OpenWritable(fullPath))
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                {
                    totalBytesRead += bytesRead;
                    var contentLength = response.Content.Headers.ContentLength != null ?
                        (long) response.Content.Headers.ContentLength :
                        totalBytesRead;
                    await writeableStream.WriteAsync(buffer, 0, bytesRead);
                    await statusController.UpdateProgressAsync(
                        status,
                        totalBytesRead,
                        contentLength,
                        filename,
                        DataUnits.Bytes);
                }
            }
        }

        public async Task DownloadFileFromSourceAsync(string sourceUri, string destination, IStatus status)
        {
            var downloadEntryTask = await statusController.CreateAsync(status, "Download entry");
            try
            {
                using (var response = await networkController.RequestResponseAsync(downloadEntryTask, HttpMethod.Get, sourceUri))
                    await DownloadFileFromResponseAsync(response, destination, downloadEntryTask);
            }
            catch (Exception ex)
            {
                await statusController.WarnAsync(downloadEntryTask, $"{sourceUri}: {ex.Message}");
            }
            finally
            {
                await statusController.CompleteAsync(downloadEntryTask);
            }
        }
    }
}
