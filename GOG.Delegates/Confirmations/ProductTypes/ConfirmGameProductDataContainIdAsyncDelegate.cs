using Attributes;
using Delegates.Confirmations;
using GOG.Models;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Confirmations.ProductTypes
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