using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Confirmations;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmApiProductsContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetApiProductByIdAsyncDelegate))]
        public ConfirmApiProductsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ApiProduct, long> getApiProductByIdAsyncDelegate) :
            base(getApiProductByIdAsyncDelegate)
        {
            // ...
        }
    }
}