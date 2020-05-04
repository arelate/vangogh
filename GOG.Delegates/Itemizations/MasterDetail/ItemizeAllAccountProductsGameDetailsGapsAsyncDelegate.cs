using Attributes;
using Delegates.Itemizations.MasterDetail;
using GOG.Delegates.Confirmations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations.MasterDetail
{
    public class ItemizeAllAccountProductsGameDetailsGapsAsyncDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(ProductTypes.ItemizeAllAccountProductsAsyncDelegate),
            typeof(ConfirmGameDetailsContainIdAsyncDelegate))]
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