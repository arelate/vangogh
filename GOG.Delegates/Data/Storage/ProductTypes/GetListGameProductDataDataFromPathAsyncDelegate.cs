using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class
        GetListGameProductDataDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            typeof(GetListGameProductDataDataAsyncDelegate),
            typeof(GetGameProductDataPathDelegate))]
        public GetListGameProductDataDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<GameProductData>,string> getListGameProductDataDataAsyncDelegate,
            IGetPathDelegate getGameProductDataPathDelegate) :
            base(
                getListGameProductDataDataAsyncDelegate,
                getGameProductDataPathDelegate)
        {
            // ...
        }
    }
}