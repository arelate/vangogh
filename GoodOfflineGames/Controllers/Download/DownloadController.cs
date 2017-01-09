using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Interfaces.Download;
using Interfaces.Network;
using Interfaces.Stream;
using Interfaces.TaskStatus;
using Interfaces.File;

namespace Controllers.Download
{
    public class DownloadController : IDownloadController
    {
        private INetworkController networkController;
        private IStreamController streamController;
        private IFileController fileController;
        private ITaskStatusController taskStatusController;

        public DownloadController(
            INetworkController networkController,
            IStreamController streamController,
            IFileController fileController,
            ITaskStatusController taskStatusController)
        {
            this.networkController = networkController;
            this.streamController = streamController;
            this.fileController = fileController;

            this.taskStatusController = taskStatusController;
        }

        public async Task DownloadFileAsync(HttpResponseMessage response, string destination, ITaskStatus taskStatus)
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
                return;

            using (var writeableStream = streamController.OpenWritable(fullPath))
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                {
                    totalBytesRead += bytesRead;
                    await writeableStream.WriteAsync(buffer, 0, bytesRead);
                    taskStatusController.UpdateProgress(
                        taskStatus, 
                        totalBytesRead, 
                        (long) response.Content.Headers.ContentLength,
                        filename,
                        "byte(s)");
                }
            }
        }
    }
}
