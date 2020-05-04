using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListGameProductDataDataAsyncDelegate : GetJSONDataAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToListGameProductDataDelegate))]
        public GetListGameProductDataDataAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<GameProductData>> convertJSONToListGameProductDataDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListGameProductDataDelegate)
        {
            // ...
        }
    }
}