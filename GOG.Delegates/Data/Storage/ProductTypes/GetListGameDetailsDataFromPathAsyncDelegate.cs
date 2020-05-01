using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListGameDetailsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            typeof(GetListGameDetailsDataAsyncDelegate),
            typeof(GetGameDetailsPathDelegate))]
        public GetListGameDetailsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<GameDetails>,string> getListGameDetailsDataAsyncDelegate,
            IGetPathDelegate getGameDetailsPathDelegate) :
            base(
                getListGameDetailsDataAsyncDelegate,
                getGameDetailsPathDelegate)
        {
            // ...
        }
    }
}