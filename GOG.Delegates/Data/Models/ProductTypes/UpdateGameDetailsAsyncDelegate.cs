using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Delegates.Confirmations.ProductTypes;
using GOG.Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateGameDetailsAsyncDelegate: UpdateDataAsyncDelegate<GameDetails>
    {
        [Dependencies(
            typeof(DeleteGameDetailsAsyncDelegate),
            typeof(ConvertGameDetailsToIndexDelegate),
            typeof(ConfirmGameDetailsContainIdAsyncDelegate),
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate))]
        public UpdateGameDetailsAsyncDelegate(
            IDeleteAsyncDelegate<GameDetails> deleteGameDetailsAsyncDelegate, 
            IConvertDelegate<GameDetails, long> convertGameDetailsToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmGameDetailsContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<GameDetails>, string> getGameDetailsAsyncDelegate) : 
            base(
                deleteGameDetailsAsyncDelegate, 
                convertGameDetailsToIndexDelegate,
                confirmGameDetailsContainsIdAsyncDelegate, 
                getGameDetailsAsyncDelegate)
        {
            // ...
        }
    }
}