using Attributes;
using Delegates.Confirmations;
using GOG.Models;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Confirmations.ProductTypes
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