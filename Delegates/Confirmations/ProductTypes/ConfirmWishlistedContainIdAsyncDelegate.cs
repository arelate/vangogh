using Attributes;
using Delegates.Data.Models.ProductTypes;
using Interfaces.Delegates.Data;

namespace Delegates.Confirmations.ProductTypes
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