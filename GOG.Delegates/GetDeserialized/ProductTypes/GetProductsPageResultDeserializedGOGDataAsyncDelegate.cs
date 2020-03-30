using System.Collections.Generic;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetProductsPageResultDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            "Delegates.Convert.Network.ConvertUriDictionaryParametersToUriDelegate,Delegates",
            "Delegates.GetData.Network.GetUriDataAsyncDelegate,Delegates",
            "GOG.Delegates.Itemize.ItemizeGOGDataDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToProductsPageResultDelegate,GOG.Delegates")]
        public GetProductsPageResultDeserializedGOGDataAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,            
            IGetDataAsyncDelegate<string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string,ProductsPageResult> convertJSONToProductsPageResult) :
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