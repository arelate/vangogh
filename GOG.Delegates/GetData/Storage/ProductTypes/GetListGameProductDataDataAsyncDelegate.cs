using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListGameProductDataDataAsyncDelegate : GetJSONDataAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToListGameProductDataDelegate,GOG.Delegates")]
        public GetListGameProductDataDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<GameProductData>> convertJSONToListGameProductDataDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListGameProductDataDelegate)
        {
            // ...
        }
    }
}