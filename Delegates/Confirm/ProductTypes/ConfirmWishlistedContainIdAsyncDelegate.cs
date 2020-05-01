using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;
using Delegates.Data.Models.ProductTypes;

namespace Delegates.Confirm.ProductTypes
{
    public class ConfirmWishlistedContainIdAsyncDelegate: ConfirmDataContainsIdAsyncDelegate<long>
    {
        [Dependencies(
            typeof(GetWishlistedByIdAsyncDelegate))]
        public ConfirmWishlistedContainIdAsyncDelegate(
            IGetDataAsyncDelegate<long, long> getDataByIdAsyncDelegate) : 
            base(getDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}