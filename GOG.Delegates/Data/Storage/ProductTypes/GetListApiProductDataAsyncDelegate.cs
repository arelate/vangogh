using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListApiProductDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            "Delegates.Data.Storage.GetStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToListApiProductDelegate,GOG.Delegates")]
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