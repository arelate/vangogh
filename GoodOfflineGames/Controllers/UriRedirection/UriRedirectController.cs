using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.UriRedirection;
using Interfaces.Network;

using Models.Uris;

namespace Controllers.UriRedirection
{
    public class UriRedirectController : IUriRedirectController
    {
        private INetworkController networkController;

        public UriRedirectController(INetworkController networkController)
        {
            this.networkController = networkController;
        }

        public async Task<string> GetUriRedirect(string uri)
        {
            var response = await networkController.GetResponse(
                HttpMethod.Get,
                string.Format( 
                    Uris.Paths.ProductFiles.FullUriTemplate,
                    uri));
            return response.RequestMessage.RequestUri.ToString();
        }
    }
}
