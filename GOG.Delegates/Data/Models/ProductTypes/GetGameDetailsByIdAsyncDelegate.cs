using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetGameDetailsByIdAsyncDelegate: GetDataByIdAsyncDelegate<GameDetails>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate),
            typeof(GOG.Delegates.Collections.ProductTypes.FindGameDetailsDelegate),
            typeof(GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsToIndexDelegate))]
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