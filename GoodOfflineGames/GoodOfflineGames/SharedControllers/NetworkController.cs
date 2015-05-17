using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace GOG
{
    public class NetworkController
    {
        public const string postMethod = "POST";
        public const string getMethod = "GET";

        public const string acceptHtml = "text/html, application/xhtml+xml, */*";
        public const string acceptJson = "application/json, text/plain, */*";

        private static HttpWebRequest request = null;
        private static CookieContainer sharedCookies = new CookieContainer();

        private static async Task<WebResponse> RequestResponse(
            string uri,
            string method = getMethod,
            string data = null,
            string accept = acceptJson)
        {
            Uri requestUri = new Uri(uri);

            request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            request.CookieContainer = sharedCookies;

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SetTcpKeepAlive(true, 10000, 10000);

            // using IE11 default UA string
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Method = method;
            request.Accept = accept;

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
                response = await Task.Factory.FromAsync<WebResponse>(
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

        public static string CombineQueryParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return string.Empty;

            List<string> parametersStrings = new List<string>(parameters.Count);

            foreach (string key in parameters.Keys)
            {
                parametersStrings.Add(
                    Uri.EscapeDataString(key) +
                    Separators.KeyValueSeparator +
                    Uri.EscapeDataString(parameters[key]));
            }

            return string.Join(Separators.QueryStringParameters, parametersStrings);
        }

        private static string CombineUri(string baseUri, Dictionary<string, string> parameters)
        {
            return (parameters == null) ?
                baseUri :
                baseUri +
                Separators.QueryString +
                CombineQueryParameters(parameters);
        }

        public async static Task RequestFile(
            string fromUri,
            string toFile,
            IStreamWritableController streamWriteableController)
        {
            WebResponse response = await RequestResponse(fromUri);
            int bufferSize = 64 * 1024; // 64K
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;

            if (response == null) return;

            using (Stream writeableStream = streamWriteableController.OpenWritable(toFile))
            using (Stream responseStream = response.GetResponseStream())
                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    await writeableStream.WriteAsync(buffer, 0, bytesRead);
        }

        public async static Task<string> RequestString(
            string baseUri,
            Dictionary<string, string> parameters = null,
            string method = getMethod,
            string data = null,
            string accept = acceptJson)
        {
            string uri = CombineUri(baseUri, parameters);

            WebResponse response = await RequestResponse(uri, method, data, accept);

            if (response == null) return null;

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                return await reader.ReadToEndAsync();
        }

        public async static Task<T> RequestData<T>(
            string baseUri,
            Dictionary<string, string> parameters = null)
        {
            var dataString = await RequestString(baseUri, parameters);
            return JSONController.Parse<T>(dataString);
        }
    }
}
