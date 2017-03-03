using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using System;

using Interfaces.FileDownload;
using Interfaces.Network;
using Interfaces.Stream;
using Interfaces.TaskStatus;
using Interfaces.File;

using Models.Units;

namespace Controllers.FileDownload
{
    public class FileDownloadController : IFileDownloadController
    {
        private INetworkController networkController;
        private IStreamController streamController;
        private IFileController fileController;
        private ITaskStatusController taskStatusController;

        public FileDownloadController(
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

        public async Task DownloadFileFromResponseAsync(HttpResponseMessage response, string destination, ITaskStatus taskStatus)
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
                taskStatusController.Warn(
                    taskStatus, 
                    "File resolved to the path {0} already exists and matches response size - won't be redownloading it again", 
                    fullPath);

                return;
            }

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
                        (long)response.Content.Headers.ContentLength,
                        filename,
                        DataUnits.Bytes);
                }
            }
        }

        public async Task DownloadFileFromSourceAsync(long id, string title, string sourceUri, string destination, ITaskStatus taskStatus)
        {
            var downloadEntryTask = taskStatusController.Create(taskStatus, "Download entry");
            try
            {
                using (var response = await networkController.GetResponse(HttpMethod.Head, sourceUri))
                    await DownloadFileFromResponseAsync(response, destination, downloadEntryTask);
            }
            catch (Exception ex)
            {
                taskStatusController.Warn(downloadEntryTask,
                    string.Format(
                        "{0}: {1}",
                        sourceUri,
                        ex.Message));
            }
            finally
            {
                taskStatusController.Complete(downloadEntryTask);
            }
        }
    }
}
