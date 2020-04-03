using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Delegates.GetData.Storage;

using GOG.Models;

namespace GOG.Delegates.GetData.Storage.ProductTypes
{
    public class GetListApiProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            "GOG.Delegates.GetData.Storage.ProductTypes.GetListApiProductDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetApiProductsPathDelegate,Delegates")]
        public GetListApiProductDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ApiProduct>> getListApiProductDataAsyncDelegate, 
            IGetPathDelegate getApiProductsPathDelegate) : 
            base(
                getListApiProductDataAsyncDelegate, 
                getApiProductsPathDelegate)
        {
            // ...
        }
    }
}