using Interfaces.Controllers.Data;
using Interfaces.Models.Dependencies;

using Delegates.Itemize.MasterDetail;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Itemize.MasterDetail
{
    public class ItemizeAllProductsGameProductDataGapsAsyncDelegatepsDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<Product,GameProductData>
    {
        [Dependencies(
            DependencyContext.Default,
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "GOG.Controllers.Data.ProductTypes.GameProductDataDataController,GOG.Controllers")]
        public ItemizeAllProductsGameProductDataGapsAsyncDelegatepsDelegate(
            IDataController<Product> productsDataController,
            IDataController<GameProductData> gameProductDataDataController):
            base(
                productsDataController,
                gameProductDataDataController)
                {
                    // ...
                }
    }
}
