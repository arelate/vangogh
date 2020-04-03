using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListGameDetailsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            "GOG.Delegates.GetData.Storage.ProductTypes.GetListGameDetailsDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetGameDetailsPathDelegate,Delegates")]
        public GetListGameDetailsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<GameDetails>> getListGameDetailsDataAsyncDelegate, 
            IGetPathDelegate getGameDetailssPathDelegate) : 
            base(
                getListGameDetailsDataAsyncDelegate, 
                getGameDetailssPathDelegate)
        {
            // ...
        }
    }
}