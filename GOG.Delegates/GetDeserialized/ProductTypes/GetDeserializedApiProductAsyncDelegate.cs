using Interfaces.Controllers.Network;

using Interfaces.Controllers.Serialization;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedApiProductAsyncDelegate : GetDeserializedProductCoreAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Network.NetworkController,Controllers",
            Dependencies.JSONSerializationController)]
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