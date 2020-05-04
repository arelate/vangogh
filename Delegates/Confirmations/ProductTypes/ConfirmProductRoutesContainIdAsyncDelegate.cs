using Attributes;
using Delegates.Data.Models.ProductTypes;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Confirmations.ProductTypes
{
    public class ConfirmProductRoutesContainIdAsyncDelegate: 
        ConfirmDataContainsIdAsyncDelegate<ProductRoutes>
    {
        [Dependencies(
            typeof(GetProductRoutesByIdAsyncDelegate))]
        public ConfirmProductRoutesContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ProductRoutes, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}