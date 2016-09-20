using System.Threading.Tasks;

using Interfaces.UriRedirect;
using Interfaces.Network;

using Models.Uris;

namespace Controllers.UriResolution
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
                string.Format( 
                    Uris.Paths.ProductFiles.FullUriTemplate,
                    uri));
            return response.RequestMessage.RequestUri.ToString();
        }
    }
}
