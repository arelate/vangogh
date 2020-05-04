using System.Collections.Generic;
using Attributes;
using Delegates.Convert.Uri;
using GOG.Models;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemize;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetProductsPageResultDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(Network.GetUriDataPolitelyAsyncDelegate),
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