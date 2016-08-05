using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.RequestPage;

namespace Controllers.RequestPage
{
    public class RequestPageController: IRequestPageController
    {
        private IStringNetworkController stringNetworkingController;
        private const string pageQueryParameter = "page";

        public RequestPageController(IStringNetworkController stringNetworkingController)
        {
            this.stringNetworkingController = stringNetworkingController;
        }

        public async Task<string> RequestPage(
            string uri,
            IDictionary<string, string> parameters,
            int page)
        {
            if (!parameters.Keys.Contains(pageQueryParameter))
                parameters.Add(pageQueryParameter, page.ToString());

            parameters[pageQueryParameter] = page.ToString();

            return await stringNetworkingController.GetString(uri, parameters);
        }
    }
}
