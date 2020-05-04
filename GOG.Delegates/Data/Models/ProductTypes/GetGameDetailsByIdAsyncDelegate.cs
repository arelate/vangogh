using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;
using GOG.Delegates.Collections.ProductTypes;
using GOG.Delegates.Conversions.ProductTypes;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetGameDetailsByIdAsyncDelegate: GetDataByIdAsyncDelegate<GameDetails>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate),
            typeof(FindGameDetailsDelegate),
            typeof(ConvertGameDetailsToIndexDelegate))]
        public GetGameDetailsByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<GameDetails>, string> getListGameDetailsAsyncDelegate, 
            IFindDelegate<GameDetails> findDelegate, 
            IConvertDelegate<GameDetails, long> convertProductToIndexDelegate) : 
            base(
                getListGameDetailsAsyncDelegate, 
                findDelegate, 
                convertProductToIndexDelegate)
        {
            // ...
        }
    }
}