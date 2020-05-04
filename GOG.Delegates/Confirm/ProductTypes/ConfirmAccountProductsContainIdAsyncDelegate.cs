using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Confirmations;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmAccountProductsContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetAccountProductByIdAsyncDelegate))]
        public ConfirmAccountProductsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<AccountProduct, long> getAccountProductByIdAsyncDelegate) :
            base(getAccountProductByIdAsyncDelegate)
        {
            // ...
        }
    }
}