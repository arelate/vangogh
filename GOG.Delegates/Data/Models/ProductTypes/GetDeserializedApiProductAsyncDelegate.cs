using Attributes;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetDeserializedApiProductAsyncDelegate : GetDeserializedProductCoreAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(Network.GetUriDataPolitelyAsyncDelegate),
            typeof(ConvertJSONToApiProductDelegate))]
        public GetDeserializedApiProductAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IConvertDelegate<string, ApiProduct> convertJSONToApiProductDelegate) :
            base(
                getUriDataAsyncDelegate,
                convertJSONToApiProductDelegate)
        {
            // ...
        }
    }
}