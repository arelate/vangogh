using System.Collections.Generic;
using Interfaces.Delegates.Values;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Activities;
using Attributes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Delegates.Activities;
using Delegates.Data.Network;
using Delegates.Convert.Uri;
using Delegates.Values.QueryParameters.ProductTypes;
using Delegates.Values.Uri.ProductTypes;

namespace GOG.Delegates.GetPageResults.ProductTypes
{
    public class GetProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            typeof(GetProductsUpdateUriDelegate),
            typeof(GetProductsUpdateQueryParametersDelegate),
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(GetUriDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToProductsPageResultDelegate),
            typeof(StartDelegate),
            typeof(SetProgressDelegate),
            typeof(CompleteDelegate))]
        public GetProductsPageResultsAsyncDelegate(
            IGetValueDelegate<string, string> getProductsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>, string> getProductsQueryUpdateQueryParameters,
            IConvertDelegate<(string, IDictionary<string, string>), string>
                convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,            
            IConvertDelegate<string, ProductsPageResult> convertJSONToProductsPageResultDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getProductsUpdateUriDelegate,
                getProductsQueryUpdateQueryParameters,
                convertUriParametersToUriDelegate,
                getUriDataAsyncDelegate,
                convertJSONToProductsPageResultDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}