using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                string.Format( 
                    Uris.Paths.ProductFiles.FullUriTemplate,
                    uri));
            return response.RequestMessage.RequestUri.ToString();
        }
    }
}
