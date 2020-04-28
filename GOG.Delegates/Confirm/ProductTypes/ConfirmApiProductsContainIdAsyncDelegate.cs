using Delegates.Confirm;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmApiProductsContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Models.ProductTypes.GetApiProductByIdAsyncDelegate,GOG.Delegates")]
        public ConfirmApiProductsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ApiProduct, long> getApiProductByIdAsyncDelegate) :
            base(getApiProductByIdAsyncDelegate)
        {
            // ...
        }
    }
}