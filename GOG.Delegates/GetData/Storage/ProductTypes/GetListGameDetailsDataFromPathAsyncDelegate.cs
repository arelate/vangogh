using System.Collections.Generic;

using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListGameDetailsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataAsyncDelegate,GOG.Delegates",
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