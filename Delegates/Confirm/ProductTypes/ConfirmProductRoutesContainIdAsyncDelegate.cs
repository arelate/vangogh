using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Confirm.ProductTypes
{
    public class ConfirmProductRoutesContainIdAsyncDelegate: 
        ConfirmDataContainsIdAsyncDelegate<ProductRoutes>
    {
        [Dependencies(
            "Delegates.Data.Models.ProductTypes.GetProductRoutesByIdAsyncDelegate,Delegates")]
        public ConfirmProductRoutesContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ProductRoutes, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}