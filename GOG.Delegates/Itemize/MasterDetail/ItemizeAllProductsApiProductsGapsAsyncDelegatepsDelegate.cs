using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Confirm;
using Delegates.Itemize.MasterDetail;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Itemize.MasterDetail
{
    public class ItemizeAllProductsApiProductsGapsAsyncDelegatepsDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<Product>
    {
        [Dependencies(
            "GOG.Delegates.Itemize.ProductTypes.ItemizeAllProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmApiProductsContainIdAsyncDelegate,GOG.Delegates")]
        public ItemizeAllProductsApiProductsGapsAsyncDelegatepsDelegate(
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