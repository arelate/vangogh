using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Confirm;
using Delegates.Itemize.MasterDetail;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Itemize.MasterDetail
{
    public class ItemizeAllProductsApiProductsGapsAsyncDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(ProductTypes.ItemizeAllProductsAsyncDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmApiProductsContainIdAsyncDelegate))]
        public ItemizeAllProductsApiProductsGapsAsyncDelegate(
            IItemizeAllAsyncDelegate<Product> itemizeAllProductsAsyncDelegate,
            IConfirmAsyncDelegate<long> confirmApiProductsContainIdAsyncDelegate) :
            base(
                itemizeAllProductsAsyncDelegate,
                confirmApiProductsContainIdAsyncDelegate)
        {
            // ...
        }
    }
}