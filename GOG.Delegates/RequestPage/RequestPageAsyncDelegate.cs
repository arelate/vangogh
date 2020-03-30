using System.Collections.Generic;
using System.Threading.Tasks;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.Convert;

using GOG.Interfaces.Delegates.RequestPage;

namespace GOG.Delegates.RequestPage
{
    public class RequestPageAsyncDelegate: IRequestPageAsyncDelegate
    {
        private readonly IConvertDelegate<(string, IDictionary<string,string>), string> convertUriParametersToUriDelegate;
        private readonly IGetDataAsyncDelegate<string> getUriDataAsyncDelegate;

        const string pageQueryParameter = "page";

        [Dependencies(
            "Controllers.Network.NetworkController,Controllers")]
        public RequestPageAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string,string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string> getUriDataAsyncDelegate)
        {
            this.convertUriParametersToUriDelegate = convertUriParametersToUriDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
        }

        public async Task<string> RequestPageAsync(
            string uri,
            IDictionary<string, string> parameters,
            int page)
        {
            if (!parameters.Keys.Contains(pageQueryParameter))
                parameters.Add(pageQueryParameter, page.ToString());

            parameters[pageQueryParameter] = page.ToString();

            var uriParameters = convertUriParametersToUriDelegate.Convert((uri, parameters));
            var pageResponse = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);

            return pageResponse;
        }
    }
}
