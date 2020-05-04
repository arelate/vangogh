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
    public class DeleteGameDetailsAsyncDelegate: DeleteAsyncDelegate<GameDetails>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate),
            typeof(ConvertGameDetailsToIndexDelegate),
            typeof(ConfirmGameDetailsContainIdAsyncDelegate))]
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