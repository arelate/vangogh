using Attributes;
using Delegates.Itemizations.MasterDetail;
using GOG.Delegates.Confirmations.ProductTypes;
using GOG.Models;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Itemizations.MasterDetail
{
    public class ItemizeAllProductsGameProductDataGapsAsyncDelegate :
        ItemizeAllMasterDetailsGapsAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(ProductTypes.ItemizeAllProductsAsyncDelegate),
            typeof(ConfirmGameProductDataContainIdAsyncDelegate))]
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