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
    public class GetProductsPageResultsAsyncDelegate : GetPageResultsAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            typeof(GetProductsUpdateUriDelegate),
            typeof(GetProductsUpdateQueryParametersDelegate),
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(GetUriDataAsyncDelegate),
            typeof(ConvertJSONToProductsPageResultDelegate),
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