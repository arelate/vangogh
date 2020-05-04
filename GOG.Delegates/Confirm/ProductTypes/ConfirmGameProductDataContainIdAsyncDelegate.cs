using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Confirmations;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmGameProductDataContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<GameProductData>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetGameProductDataByIdAsyncDelegate))]
        public ConfirmGameProductDataContainIdAsyncDelegate(
            IGetDataAsyncDelegate<GameProductData, long> getGameProductDataByIdAsyncDelegate) :
            base(getGameProductDataByIdAsyncDelegate)
        {
            // ...
        }
    }
}