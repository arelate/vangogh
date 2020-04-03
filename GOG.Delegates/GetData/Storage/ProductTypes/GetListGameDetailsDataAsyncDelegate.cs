using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListGameDetailsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToListGameDetailsDelegate,GOG.Delegates")]
        public GetListGameDetailsDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<GameDetails>> convertJSONToListGameDetailsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListGameDetailsDelegate)
        {
            // ...
        }
    }
}