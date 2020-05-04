using Attributes;
using Delegates.Confirmations;
using GOG.Models;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Confirmations.ProductTypes
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