using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Confirm.ProductTypes
{
    public class ConfirmValidationResultsContainIdAsyncDelegate: 
        ConfirmDataContainsIdAsyncDelegate<ValidationResults>
    {
        [Dependencies(
            "Delegates.Data.Models.ProductTypes.GetValidationResultsByIdAsyncDelegate,Delegates")]
        public ConfirmValidationResultsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<ValidationResults, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}