using Interfaces.Controllers.Network;
using Interfaces.Controllers.Serialization;

using Interfaces.Delegates.Itemize;

using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetGOGDataDeserializedGOGDataAsyncDelegate :
        GetDeserializedGOGDataAsyncDelegate<GOGData>
    {
        public GetGOGDataDeserializedGOGDataAsyncDelegate(
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