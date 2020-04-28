using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class UpdateGameDetailsAsyncDelegate: UpdateDataAsyncDelegate<GameDetails>
    {
        [Dependencies(
            "GOG.Delegates.Data.Models.ProductTypes.DeleteGameDetailsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsToIndexDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmGameDetailsContainIdAsyncDelegate.GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate,GOG.Delegates")]
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