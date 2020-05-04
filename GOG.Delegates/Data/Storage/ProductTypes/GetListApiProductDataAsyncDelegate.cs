using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListApiProductDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(ConvertJSONToListApiProductDelegate))]
        public GetListApiProductDataAsyncDelegate(
            IGetDataAsyncDelegate<string,string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ApiProduct>> convertJSONToListApiProductDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListApiProductDelegate)
        {
            // ...
        }
    }
}