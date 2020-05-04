using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Models.Uris;
using Models.QueryParameters;
using Attributes;
using Delegates.Data.Network;
using Delegates.Activities;
using Delegates.Conversions.Uris;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Authorization
{
    public class GetAuthenticationTokenDataAsyncDelegate: IGetDataAsyncDelegate<string, string>
    {
        private readonly IConvertDelegate<(string, IDictionary<string, string>), string>
            convertUriParametersToUriDelegate;
        private readonly IGetDataAsyncDelegate<string, string> getUriDataAsyncDelegate;
        
        private readonly IStartDelegate startDelegate;
        private readonly ICompleteDelegate completeDelegate;
        
        [Dependencies(
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(GetUriDataAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public GetAuthenticationTokenDataAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string>
                convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string, string> getUriDataAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.convertUriParametersToUriDelegate = convertUriParametersToUriDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.startDelegate = startDelegate;
            this.completeDelegate = completeDelegate;
        }
        public async Task<string> GetDataAsync(string uri)
        {
            startDelegate.Start("Get authorization token response");

            var uriParameters = convertUriParametersToUriDelegate.Convert((
                Uris.Endpoints.Authentication.Auth,
                QueryParametersCollections.Authenticate));
            // request authorization token
            var authResponse = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);

            completeDelegate.Complete();

            return authResponse;
        }
    }
}