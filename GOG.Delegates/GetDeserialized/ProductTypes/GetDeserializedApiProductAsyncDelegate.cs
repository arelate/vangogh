using Interfaces.Controllers.Network;

using Interfaces.Delegates.Convert;


using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedApiProductAsyncDelegate : GetDeserializedProductCoreAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "Controllers.Network.NetworkController,Controllers",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToApiProductDelegate,GOG.Delegates")]
        public GetDeserializedApiProductAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            IConvertDelegate<string, ApiProduct> convertJSONToApiProductDelegate) :
            base(
                getResourceAsyncDelegate,
                convertJSONToApiProductDelegate)
        {
            // ...
        }
    }
}