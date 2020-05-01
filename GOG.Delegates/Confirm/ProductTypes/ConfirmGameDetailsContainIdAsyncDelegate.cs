using Delegates.Confirm;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmGameDetailsContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<GameDetails>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Models.ProductTypes.GetGameDetailsByIdAsyncDelegate))]
        public ConfirmGameDetailsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate) :
            base(getGameDetailsByIdAsyncDelegate)
        {
            // ...
        }
    }
}