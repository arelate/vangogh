using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

using Interfaces.IO.Stream;
using Interfaces.IO.File;
using Interfaces.Console;
using Interfaces.Network;

namespace Controllers.Network
{
    public sealed class NetworkController :
        IRequestFileDelegate,
        IStringNetworkController,
        IDisposable
    {
        private HttpClient client;
        private IUriController uriController;

        public NetworkController(IUriController uriController)
        {
            client = new HttpClient();
            this.uriController = uriController;
        }

        public async Task<Tuple<bool, Uri>> RequestFile(
            string fromUri,
            string toPath,
            IOpenWritableDelegate openWriteableDelegate,
            IFileController fileController = null,
            IDownloadProgressReportingController downloadProgressReportingController = null,
            IConsoleController consoleController = null)
        {
            using (var response = await client.GetAsync(fromUri,
                HttpCompletionOption.ResponseHeadersRead))
            {
                var totalBytes = response.Content.Headers.ContentLength;
                if (totalBytes == null) totalBytes = 0;

                var requestUri = response.RequestMessage.RequestUri;
                var filename = requestUri.Segments.Last();

                if (!response.IsSuccessStatusCode)
                {
                    if (consoleController != null)
                        consoleController.Write("ERROR {0}. Couldn't download file.", MessageType.Error, response.StatusCode);

                    return new Tuple<bool, Uri>(false, requestUri);
                }

                var fullPath = Path.Combine(toPath, filename);

                int bufferSize = 1024 * 1024; // 1M
                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;
                long totalBytesRead = 0;

                if (fileController != null &&
                    fileController.FileExists(fullPath) &&
                    fileController.GetSize(fullPath) == totalBytes)
                {
                    // file already exists and has same length - assume it's downloaded
                    if (consoleController != null)
                        consoleController.Write("The file with the same name and size already exists.", MessageType.Success);

                    return new Tuple<bool, Uri>(true, requestUri);
                }

                downloadProgressReportingController?.Initialize();

                using (Stream writeableStream = openWriteableDelegate.OpenWritable(fullPath))
                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        totalBytesRead += bytesRead;
                        await writeableStream.WriteAsync(buffer, 0, bytesRead);
                        downloadProgressReportingController?.Report(totalBytesRead, (long)totalBytes);
                    }
                }

                downloadProgressReportingController?.Report((long)totalBytes, (long)totalBytes);

                return new Tuple<bool, Uri>(true, requestUri);
            }
        }

        public async Task<string> GetString(
            string baseUri,
            IDictionary<string, string> parameters = null)
        {
            string uri = uriController.ConcatenateUri(baseUri, parameters);

            using (var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
            {
                if (response == null) return null;

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public async Task<string> PostString(
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.ConcatenateUri(baseUri, parameters);

            var content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

            using (var response = await client.PostAsync(uri, content))
            {
                if (response == null) return null;

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public void Dispose()
        {
            if (client != null) client.Dispose();
        }
    }
}
