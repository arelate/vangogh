using System.Net.Http;
using Interfaces.Delegates.Values;
using Models.Network;

namespace Delegates.Values.Network
{
    public class GetHttpClientInstanceDelegate : IGetInstanceDelegate<HttpClient>
    {
        private HttpClient httpClient;

        public HttpClient GetInstance()
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient(
                    new HttpClientHandler
                    {
                        UseDefaultCredentials = false,
                        UseCookies = true
                    });
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                httpClient.DefaultRequestHeaders.Add(Headers.UserAgent, HeaderDefaultValues.UserAgent);
                httpClient.DefaultRequestHeaders.Add(Headers.Accept, HeaderDefaultValues.Accept);
            }

            return httpClient;
        }
    }
}