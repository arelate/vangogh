using Attributes;
using Delegates.Itemizations.MasterDetail;
using GOG.Delegates.Confirmations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations.MasterDetail
{
    public class ItemizeAllProductsApiProductsGapsAsyncDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(ProductTypes.ItemizeAllProductsAsyncDelegate),
            typeof(ConfirmApiProductsContainIdAsyncDelegate))]
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