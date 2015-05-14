using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace GOG
{
    class Network
    {
        // TODO: Port async download helper
        public const string postMethod = "POST";
        public const string getMethod = "GET";
        private const string defaultUserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
        public const string acceptHtml = "text/html, application/xhtml+xml, */*";
        public const string acceptJson = "application/json, text/javascript, */*; q=0.01";

        private static string contentTypeXWWWFormEncoded = "application/x-www-form-urlencoded";

        private static HttpWebRequest request = null;
        private static CookieContainer sharedCookies = new CookieContainer();

        private static async Task<WebResponse> RequestResponse(string uri, string method, string data, string accept, string referer, Dictionary<string, string> additionalHeaders = null)
        {
            Uri requestUri = new Uri(uri);

            request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            request.CookieContainer = sharedCookies;

            if (additionalHeaders != null)
            {
                foreach (KeyValuePair<string, string> keyValue in additionalHeaders)
                {
                    request.Headers.Add(keyValue.Key, keyValue.Value);
                }
            }

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SetTcpKeepAlive(true, 10000, 10000);

            request.UserAgent = defaultUserAgent;

            if (!string.IsNullOrEmpty(referer))
            {
                request.Referer = referer;
            }

            request.Accept = accept;

            if (method == string.Empty) method = getMethod;
            request.Method = method;

            if (!string.IsNullOrEmpty(data) &&
                method == postMethod)
            {
                var postData = Encoding.ASCII.GetBytes(data);

                request.ContentType = contentTypeXWWWFormEncoded;
                request.ContentLength = postData.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
            }

            WebResponse response = null;

            try
            {
                response = await Task.Factory.FromAsync<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null);
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return response;

        }

        public static string CombineQueryParameters(Dictionary<string, string> parameters)
        {

            if (parameters == null)
            {
                return string.Empty;
            }

            List<string> parametersStrings = new List<string>(parameters.Count);

            foreach (string key in parameters.Keys)
            {
                parametersStrings.Add(Uri.EscapeDataString(key) + Separators.KeyValueSeparator + Uri.EscapeDataString(parameters[key]));
            }

            return string.Join(Separators.QueryStringParameters, parametersStrings);
        }

        private static string CombineUri(string baseUri, Dictionary<string, string> parameters)
        {
            string uri = baseUri;

            uri += Separators.QueryString + CombineQueryParameters(parameters);

            return uri;
        }

        public async static Task<string> Request(
            string baseUri,
            Dictionary<string, string> parameters = null, 
            string method = getMethod, 
            string data = null, 
            string accept = acceptHtml, 
            string referer = null, 
            Dictionary<string, string> additionalHeaders = null)
        {

            string uri = CombineUri(baseUri, parameters);

            WebResponse response = await RequestResponse(uri, method, data, accept, referer, additionalHeaders);

            if (response != null)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            else
            {
                return null;
            }
        }
    }
}
