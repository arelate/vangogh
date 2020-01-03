using System.Collections.Generic;
using System.Threading.Tasks;

using GOG.Interfaces.Delegates.RequestPage;

using Interfaces.Controllers.Network;
using Interfaces.Models.Dependencies;

using Attributes;

namespace GOG.Delegates.RequestPage
{
    public class RequestPageAsyncDelegate: IRequestPageAsyncDelegate
    {
        readonly INetworkController networkController;

        const string pageQueryParameter = "page";

        [Dependencies(
            DependencyContext.Default,
            "Controllers.Network.NetworkController,Controllers")]
        public RequestPageAsyncDelegate(
            INetworkController networkController)
        {
            this.networkController = networkController;
        }

        public async Task<string> RequestPageAsync(
            string uri,
            IDictionary<string, string> parameters,
            int page)
        {
            if (!parameters.Keys.Contains(pageQueryParameter))
                parameters.Add(pageQueryParameter, page.ToString());

            parameters[pageQueryParameter] = page.ToString();

            var pageResponse = await networkController.GetResourceAsync(uri, parameters);

            return pageResponse;
        }
    }
}
