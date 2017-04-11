using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.RequestPage;
using Interfaces.Status;

namespace Controllers.RequestPage
{
    public class RequestPageController: IRequestPageController
    {
        private INetworkController networkController;

        private const string pageQueryParameter = "page";

        public RequestPageController(
            INetworkController networkController)
        {
            this.networkController = networkController;
        }

        public async Task<string> RequestPage(
            string uri,
            IDictionary<string, string> parameters,
            int page,
            IStatus status)
        {
            if (!parameters.Keys.Contains(pageQueryParameter))
                parameters.Add(pageQueryParameter, page.ToString());

            parameters[pageQueryParameter] = page.ToString();

            var pageResponse = await networkController.Get(status, uri, parameters);

            return pageResponse;
        }
    }
}
