using System.Collections.Generic;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Convert;
using Attributes;
using GOG.Models;
using Delegates.Convert.Uri;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<GOGData>
    {
        [Dependencies(
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(Data.Network.GetUriDataRateLimitedAsyncDelegate),
            typeof(Itemize.ItemizeGOGDataDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToGOGDataDelegate))]
        public GetDeserializedGOGDataAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string, GOGData> convertJSONToGOGDataDelegate) :
            base(
                convertUriParametersToUriDelegate,
                getUriDataAsyncDelegate,
                itemizeGogDataDelegate,
                convertJSONToGOGDataDelegate)
        {
            // ...
        }
    }
}