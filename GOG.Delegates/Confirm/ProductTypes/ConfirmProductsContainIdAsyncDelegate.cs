using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Confirmations;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmProductsContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<Product>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate))]
        public ConfirmProductsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate) :
            base(getProductByIdAsyncDelegate)
        {
            // ...
        }
    }
}