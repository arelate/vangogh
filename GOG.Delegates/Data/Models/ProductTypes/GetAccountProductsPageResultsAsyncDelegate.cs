using System.Collections.Generic;
using Attributes;
using Delegates.Activities;
using Delegates.Conversions.Uris;
using Delegates.Data.Network;
using Delegates.Values.QueryParameters.ProductTypes;
using Delegates.Values.Uri.ProductTypes;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetAccountProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<AccountProductsPageResult>
    {
        [Dependencies(
            typeof(GetAccountProductsUpdateUriDelegate),
            typeof(GetAccountProductsUpdateQueryParametersDelegate),
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(GetUriDataAsyncDelegate),
            typeof(ConvertJSONToAccountProductsPageResultDelegate),
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