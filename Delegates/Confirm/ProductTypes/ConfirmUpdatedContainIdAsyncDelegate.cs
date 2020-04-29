using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Confirm.ProductTypes
{
    // TODO: This and wishlisted should be a form of passthrough
    public class ConfirmUpdatedContainIdAsyncDelegate: ConfirmDataContainsIdAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Models.ProductTypes.GetUpdatedByIdAsyncDelegate,Delegates")]
        public ConfirmUpdatedContainIdAsyncDelegate(
            IGetDataAsyncDelegate<long, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}