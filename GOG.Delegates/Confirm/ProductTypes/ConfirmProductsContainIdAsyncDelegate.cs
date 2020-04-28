using Delegates.Confirm;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmProductsContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<Product>
    {
        [Dependencies(
            "GOG.Delegates.Data.Models.ProductTypes.GetProductByIdAsyncDelegate,GOG.Delegates")]
        public ConfirmProductsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<Product, long> getProductByIdAsyncDelegate) :
            base(getProductByIdAsyncDelegate)
        {
            // ...
        }
    }
}