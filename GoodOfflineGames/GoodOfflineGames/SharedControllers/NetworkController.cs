using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using GOG.Interfaces;

namespace GOG.SharedControllers
{
    public class NetworkController :
        IFileRequestController,
        IStringNetworkController
    {
        private const string postMethod = "POST";
        private const string getMethod = "GET";

        private static HttpWebRequest request;
        private static CookieContainer sharedCookies;

        private IUriController uriController;

        public NetworkController(IUriController uriController)
        {
            request = null;
            sharedCookies = new CookieContainer();
            this.uriController = uriController;
        }

        private async Task<WebResponse> RequestResponse(
            string uri,
            string method = getMethod,
            string data = null)
        {
            Uri requestUri = new Uri(uri);

            request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            request.CookieContainer = sharedCookies;

            // some special .Net magic to avoid getting network errors
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SetTcpKeepAlive(true, 10000, 10000);

            // using IE11 default UA string
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Method = method;
            request.Accept = "application/json, text/plain, */*";

            if (!string.IsNullOrEmpty(data) &&
                method == postMethod)
            {
                var postData = Encoding.ASCII.GetBytes(data);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;

                using (var stream = request.GetRequestStream())
                    stream.Write(postData, 0, postData.Length);
            }

            WebResponse response = null;

            try
            {
                response = await Task.Factory.FromAsync(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null);
            }
            catch (WebException)
            {
                return null;
            }

            return response;
        }

        public async Task RequestFile(
            string fromUri,
            string toFile,
            IStreamWritableController streamWriteableController)
        {
            WebResponse response = await RequestResponse(fromUri);
            int bufferSize = 128 * 1024; // 128K
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;

            if (response == null) return;

            using (Stream writeableStream = streamWriteableController.OpenWritable(toFile))
            using (Stream responseStream = response.GetResponseStream())
                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    await writeableStream.WriteAsync(buffer, 0, bytesRead);
        }

        public async Task<string> RequestString(
            string baseUri,
            IDictionary<string, string> parameters = null)
        {
            string uri = uriController.CombineUri(baseUri, parameters);

            WebResponse response = await RequestResponse(uri, getMethod);

            if (response == null) return null;

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                return await reader.ReadToEndAsync();
        }

        public async Task<string> PostString(
            string baseUri,
            IDictionary<string, string> parameters = null,
            string data = null)
        {
            string uri = uriController.CombineUri(baseUri, parameters);

            WebResponse response = await RequestResponse(uri, postMethod, data);

            if (response == null) return null;

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                return await reader.ReadToEndAsync();
        }
    }
}
