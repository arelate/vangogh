using Attributes;
using Delegates.Itemizations.MasterDetail;
using GOG.Models;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Itemizations;

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