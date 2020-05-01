using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Data.Models.ProductTypes;

namespace Delegates.Confirm.ProductTypes
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