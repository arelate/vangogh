using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class DeleteGameDetailsAsyncDelegate: DeleteAsyncDelegate<GameDetails>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsToIndexDelegate),
            typeof(GOG.Delegates.Confirm.ProductTypes.ConfirmGameDetailsContainIdAsyncDelegate))]
        public DeleteGameDetailsAsyncDelegate(
            IGetDataAsyncDelegate<List<GameDetails>, string> getDataCollectionAsyncDelegate, 
            IConvertDelegate<GameDetails, long> convertProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmContainsAsyncDelegate) : 
            base(
                getDataCollectionAsyncDelegate, 
                convertProductToIndexDelegate, 
                confirmContainsAsyncDelegate)
        {
            // ...
        }
    }
}