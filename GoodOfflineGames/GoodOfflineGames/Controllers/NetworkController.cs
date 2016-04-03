using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using GOG.Interfaces;

using System.Net.Http;

namespace GOG.SharedControllers
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
            IProgress<double> progress = null,
            IConsoleController consoleController = null)
        {
            using (var response = await client.GetAsync(fromUri,
                HttpCompletionOption.ResponseHeadersRead))
            {
                var totalBytes = response.Content.Headers.ContentLength;
                var requestUri = response.RequestMessage.RequestUri;
                var filename = requestUri.Segments.Last();

                if (!response.IsSuccessStatusCode)
                {
                    if (consoleController != null)
                        consoleController.Write("HTTP error {0}. Couldn't download file.", response.StatusCode);

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
                        consoleController.Write("No need to download - latest version already available.");

                    return new Tuple<bool, Uri>(true, requestUri);
                }

                using (Stream writeableStream = openWriteableDelegate.OpenWritable(fullPath))
                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        totalBytesRead += bytesRead;
                        await writeableStream.WriteAsync(buffer, 0, bytesRead);
                        if (progress != null)
                        {
                            var percent = totalBytesRead / (double)totalBytes;
                            progress.Report(percent);
                        }
                    }
                }

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
