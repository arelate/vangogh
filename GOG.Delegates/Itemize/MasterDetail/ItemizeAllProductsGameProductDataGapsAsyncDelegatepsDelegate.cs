using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Confirm;
using Delegates.Itemize.MasterDetail;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Itemize.MasterDetail
{
    public class ItemizeAllProductsGameProductDataGapsAsyncDelegatepsDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<Product>
    {
        [Dependencies(
            "GOG.Delegates.Itemize.ProductTypes.ItemizeAllProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmGameProductDataContainIdAsyncDelegate,GOG.Delegates")]
        public ItemizeAllProductsGameProductDataGapsAsyncDelegatepsDelegate(
            IItemizeAllAsyncDelegate<Product> itemizeAllProductsAsyncDelegate,
            IConfirmAsyncDelegate<long> confirmGameProductDataContainIdAsyncDelegate) :
            base(
                itemizeAllProductsAsyncDelegate,
                confirmGameProductDataContainIdAsyncDelegate)
        {
            // ...
        }
    }
}