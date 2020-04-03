using System.Collections.Generic;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedApiProductAsyncDelegate : GetDeserializedProductCoreAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "Delegates.Convert.Network.ConvertUriDictionaryParametersToUriDelegate,Delegates",
            "GOG.Delegates.GetData.Network.GetUriDataRateLimitedAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToApiProductDelegate,GOG.Delegates")]
        public GetDeserializedApiProductAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,            
            IGetDataAsyncDelegate<string> getUriDataAsyncDelegate,
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