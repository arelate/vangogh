using Delegates.Confirm;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmAccountProductsContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate,GOG.Delegates")]
        public ConfirmAccountProductsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate) :
            base(getAccountProductByIdAsyncDelegate)
        {
            // ...
        }
    }
}