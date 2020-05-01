using System.Collections.Generic;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Models;
using Delegates.Convert.Uri;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedApiProductAsyncDelegate : GetDeserializedProductCoreAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(Data.Network.GetUriDataRateLimitedAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToApiProductDelegate))]
        public GetDeserializedApiProductAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IConvertDelegate<string, ApiProduct> convertJSONToApiProductDelegate) :
            base(
                convertUriParametersToUriDelegate,
                getUriDataAsyncDelegate,
                convertJSONToApiProductDelegate)
        {
            // ...
        }
    }
}