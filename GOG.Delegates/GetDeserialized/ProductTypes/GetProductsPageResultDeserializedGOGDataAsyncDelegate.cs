using Interfaces.Controllers.Network;
using Interfaces.Controllers.Serialization;

using Interfaces.Delegates.Itemize;

using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetProductsPageResultDeserializedGOGDataAsyncDelegate :
        GetDeserializedGOGDataAsyncDelegate<ProductsPageResult>
    {
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