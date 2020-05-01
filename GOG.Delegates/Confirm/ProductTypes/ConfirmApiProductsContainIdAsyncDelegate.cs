using Delegates.Confirm;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

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