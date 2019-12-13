using Interfaces.Controllers.Network;
using Interfaces.Controllers.Serialization;

using Interfaces.Delegates.Itemize;

using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetProductsPageResultDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            "Controllers.Network.NetworkController,Controllers",
            "GOG.Delegates.Itemize.ItemizeGOGDataDelegate,GOG.Delegates",
            Dependencies.JSONSerializationController)]
        public GetProductsPageResultDeserializedGOGDataAsyncDelegate(
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