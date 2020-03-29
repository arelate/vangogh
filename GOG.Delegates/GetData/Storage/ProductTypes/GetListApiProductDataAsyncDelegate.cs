using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListApiProductDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.System.ConvertJSONToListApiProductDelegate,GOG.Delegates")]
        public GetListApiProductDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ApiProduct>> convertJSONToListApiProductDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListApiProductDelegate)
        {
            // ...
        }
    }
}