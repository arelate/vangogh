using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Confirm.ProductTypes
{
    public class ConfirmWishlistedContainIdAsyncDelegate: ConfirmDataContainsIdAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Models.ProductTypes.GetWishlistedByIdAsyncDelegate,Delegates")]
        public ConfirmWishlistedContainIdAsyncDelegate(
            IGetDataAsyncDelegate<long, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}