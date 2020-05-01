using Delegates.Confirm;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmGameDetailsContainIdAsyncDelegate : ConfirmDataContainsIdAsyncDelegate<GameDetails>
    {
        [Dependencies(
            "GOG.Delegates.Data.Models.ProductTypes.GetGameDetailsByIdAsyncDelegate,GOG.Delegates")]
        public ConfirmGameDetailsContainIdAsyncDelegate(
            IGetDataAsyncDelegate<GameDetails, long> getGameDetailsByIdAsyncDelegate) :
            base(getGameDetailsByIdAsyncDelegate)
        {
            // ...
        }
    }
}