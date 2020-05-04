using Attributes;
using Delegates.Itemizations.MasterDetail;
using GOG.Models;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Itemizations;

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