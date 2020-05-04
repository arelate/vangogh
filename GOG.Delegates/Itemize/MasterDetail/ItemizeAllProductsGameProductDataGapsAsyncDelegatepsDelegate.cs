using Attributes;
using Delegates.Itemizations.MasterDetail;
using GOG.Models;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Itemizations;

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