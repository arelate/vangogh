using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Delegates.Values.Paths.ProductTypes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListGameDetailsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            typeof(GetListGameDetailsDataAsyncDelegate),
            typeof(GetGameDetailsPathDelegate))]
        public GetListGameDetailsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<GameDetails>,string> getListGameDetailsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getGameDetailsPathDelegate) :
            base(
                getListGameDetailsDataAsyncDelegate,
                getGameDetailsPathDelegate)
        {
            // ...
        }
    }
}