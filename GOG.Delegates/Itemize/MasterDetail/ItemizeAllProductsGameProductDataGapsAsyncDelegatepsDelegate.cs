using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Confirm;
using Delegates.Itemize.MasterDetail;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Itemize.MasterDetail
{
    public class ItemizeAllProductsGameProductDataGapsAsyncDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(ProductTypes.ItemizeAllProductsAsyncDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmGameProductDataContainIdAsyncDelegate))]
        public ItemizeAllProductsGameProductDataGapsAsyncDelegate(
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