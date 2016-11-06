using System.Threading.Tasks;
using System.Net.Http;

using Interfaces.UriResolution;
using Interfaces.Network;

using Models.Uris;

namespace Controllers.UriResolution
{
    public class UriResolutionController : IUriResolutionController
    {
        private INetworkController networkController;

        public UriResolutionController(INetworkController networkController)
        {
            this.networkController = networkController;
        }

        public async Task<string> ResolveUri(string uri)
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
