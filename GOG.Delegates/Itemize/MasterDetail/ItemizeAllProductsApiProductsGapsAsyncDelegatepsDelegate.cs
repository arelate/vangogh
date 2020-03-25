using Interfaces.Controllers.Data;


using Delegates.Itemize.MasterDetail;

using Attributes;

using GOG.Models;

namespace GOG.Delegates.Itemize.MasterDetail
{
    public class ItemizeAllProductsApiProductsGapsAsyncDelegatepsDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<Product,ApiProduct>
    {
        [Dependencies(
            "GOG.Controllers.Data.ProductTypes.ProductsDataController,GOG.Controllers",
            "GOG.Controllers.Data.ProductTypes.ApiProductsDataController,GOG.Controllers")]
        public ItemizeAllProductsApiProductsGapsAsyncDelegatepsDelegate(
            IDataController<Product> productsDataController,
            IDataController<ApiProduct> apiProductsDataController):
            base(
                productsDataController,
                apiProductsDataController)
                {
                    // ...
                }
    }
}
