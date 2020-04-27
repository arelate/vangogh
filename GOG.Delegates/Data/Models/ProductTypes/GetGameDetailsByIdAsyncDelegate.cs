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
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Collections.ProductTypes.FindGameDetailsDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsToIndexDelegate,GOG.Delegates")]
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