using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Convert;
using GOG.Interfaces.Delegates.RequestPage;

namespace GOG.Delegates.RequestPage
{
    public class RequestPageAsyncDelegate : IRequestPageAsyncDelegate
    {
        private readonly IConvertDelegate<(string, IDictionary<string, string>), string>
            convertUriParametersToUriDelegate;

        private readonly IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate;

        private const string pageQueryParameter = "page";

        [Dependencies(
            "Delegates.Convert.Network.ConvertUriDictionaryParametersToUriDelegate,Delegates",
            "GOG.Delegates.Data.Network.GetUriDataRateLimitedAsyncDelegate,GOG.Delegates")]
        public RequestPageAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate)
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