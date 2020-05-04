using Attributes;
using Delegates.Confirmations;
using GOG.Models;
using Interfaces.Delegates.Data;

namespace GOG.Delegates.Confirmations.ProductTypes
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