using System.Collections.Generic;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Activities;
using Attributes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Delegates.Activities;
using Delegates.Convert.Uri;
using Delegates.Data.Network;
using Delegates.Values.QueryParameters.ProductTypes;
using Delegates.Values.Uri.ProductTypes;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetAccountProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<AccountProductsPageResult>
    {
        [Dependencies(
            typeof(GetAccountProductsUpdateUriDelegate),
            typeof(GetAccountProductsUpdateQueryParametersDelegate),
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(GetUriDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToAccountProductsPageResultDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public GetAccountProductsPageResultsAsyncDelegate(
            IGetValueDelegate<string, string> getAccountProductsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>, string> getAccountProductsQueryUpdateQueryParameters,
            IConvertDelegate<(string, IDictionary<string, string>), string>
                convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,            
            IConvertDelegate<string, AccountProductsPageResult> convertJSONToAccountProductsPageResultDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getAccountProductsUpdateUriDelegate,
                getAccountProductsQueryUpdateQueryParameters,
                convertUriParametersToUriDelegate,
                getUriDataAsyncDelegate,
                convertJSONToAccountProductsPageResultDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}