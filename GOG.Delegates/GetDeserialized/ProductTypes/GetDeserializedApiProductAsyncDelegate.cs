using Interfaces.Controllers.Network;

using Interfaces.Controllers.Serialization;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedApiProductAsyncDelegate : GetDeserializedProductCoreAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "Controllers.Network.NetworkController,Controllers",
            "Controllers.Serialization.JSON.JSONSerializationController,Controllers")]
        public GetDeserializedApiProductAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            ISerializationController<string> serializationController) :
            base(
                getResourceAsyncDelegate,
                serializationController)
        {
            // ...
        }
    }
}