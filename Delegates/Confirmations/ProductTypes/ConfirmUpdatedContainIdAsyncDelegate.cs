using Attributes;
using Delegates.Data.Models.ProductTypes;
using Interfaces.Delegates.Data;

namespace Delegates.Confirmations.ProductTypes
{
    // TODO: This and wishlisted should be a form of passthrough
    public class ConfirmUpdatedContainIdAsyncDelegate: ConfirmDataContainsIdAsyncDelegate<long>
    {
        [Dependencies(
            typeof(GetUpdatedByIdAsyncDelegate))]
        public ConfirmUpdatedContainIdAsyncDelegate(
            IGetDataAsyncDelegate<long, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}