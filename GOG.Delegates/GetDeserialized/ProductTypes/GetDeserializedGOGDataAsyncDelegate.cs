using Interfaces.Controllers.Network;
using Interfaces.Controllers.Serialization;

using Interfaces.Delegates.Itemize;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<GOGData>
    {
        [Dependencies(
            "Controllers.Network.NetworkController,Controllers",
            "GOG.Delegates.Itemize.ItemizeGOGDataDelegate,GOG.Delegates",
            "Controllers.Serialization.JSON.JSONSerializationController,Controllers")]
        public GetDeserializedGOGDataAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            ISerializationController<string> serializationController) :
            base(
                getResourceAsyncDelegate,
                itemizeGogDataDelegate,
                serializationController)
        {
            // ...
        }
    }
}