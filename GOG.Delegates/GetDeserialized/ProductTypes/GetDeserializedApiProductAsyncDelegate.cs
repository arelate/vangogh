using System.Collections.Generic;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;
using Models.Dependencies;
using GOG.Models;
using Delegates.Convert.Network;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedApiProductAsyncDelegate : GetDeserializedProductCoreAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(GOG.Delegates.Data.Network.GetUriDataRateLimitedAsyncDelegate),
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