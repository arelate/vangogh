using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListGameDetailsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            "Delegates.Data.Storage.GetStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToListGameDetailsDelegate,GOG.Delegates")]
        public GetListGameDetailsDataAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<GameDetails>> convertJSONToListGameDetailsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListGameDetailsDelegate)
        {
            // ...
        }
    }
}