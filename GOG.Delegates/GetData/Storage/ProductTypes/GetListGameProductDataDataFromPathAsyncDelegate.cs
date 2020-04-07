using System.Collections.Generic;

using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListGameProductDataDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameProductDataDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetGameProductDataPathDelegate,Delegates")]
        public GetListGameProductDataDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<GameProductData>> getListGameProductDataDataAsyncDelegate, 
            IGetPathDelegate getGameProductDataPathDelegate) : 
            base(
                getListGameProductDataDataAsyncDelegate, 
                getGameProductDataPathDelegate)
        {
            // ...
        }
    }
}