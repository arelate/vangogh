using Attributes;
using Delegates.Confirmations;
using GOG.Models;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Confirmations.ProductTypes
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