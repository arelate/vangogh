using Interfaces.Controllers.Network;

using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    public class GetProductsPageResultDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<ProductsPageResult>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Network.NetworkController,Controllers",
            "GOG.Delegates.Itemize.ItemizeGOGDataDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToProductsPageResultDelegate,GOG.Delegates")]
        public GetProductsPageResultDeserializedGOGDataAsyncDelegate(
            IGetResourceAsyncDelegate getResourceAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string,ProductsPageResult> convertJSONToProductsPageResult) :
            base(
                getResourceAsyncDelegate,
                itemizeGogDataDelegate,
                convertJSONToProductsPageResult)
        {
            // ...
        }
    }
}