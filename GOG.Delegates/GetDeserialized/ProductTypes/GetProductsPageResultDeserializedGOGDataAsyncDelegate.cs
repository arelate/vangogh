using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Models;
using Delegates.Convert.Uri;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetProductsPageResultDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(Data.Network.GetUriDataRateLimitedAsyncDelegate),
            typeof(Itemize.ItemizeGOGDataDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToProductsPageResultDelegate))]
        public GetProductsPageResultDeserializedGOGDataAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string, ProductsPageResult> convertJSONToProductsPageResult) :
            base(
                convertUriParametersToUriDelegate,
                getUriDataAsyncDelegate,
                itemizeGogDataDelegate,
                convertJSONToProductsPageResult)
        {
            // ...
        }
    }
}