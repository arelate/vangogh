using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Confirm;
using Delegates.Itemize.MasterDetail;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Itemize.MasterDetail
{
    public class ItemizeAllAccountProductsGameDetailsGapsAsyncDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(ProductTypes.ItemizeAllAccountProductsAsyncDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmGameDetailsContainIdAsyncDelegate))]
        public ItemizeAllAccountProductsGameDetailsGapsAsyncDelegate(
            IItemizeAllAsyncDelegate<AccountProduct> itemizeAllAccountProductsAsyncDelegate,
            IConfirmAsyncDelegate<long> confirmGameDetailsContainsIdAsyncDelegate) :
            base(
                itemizeAllAccountProductsAsyncDelegate,
                confirmGameDetailsContainsIdAsyncDelegate)
        {
            // ...
        }
    }
}