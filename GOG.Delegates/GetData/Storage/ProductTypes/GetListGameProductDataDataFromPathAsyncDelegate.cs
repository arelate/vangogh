using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListGameProductDataDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            "GOG.Delegates.GetData.Storage.ProductTypes.GetListGameProductDataDataAsyncDelegate,GOG.Delegates",
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