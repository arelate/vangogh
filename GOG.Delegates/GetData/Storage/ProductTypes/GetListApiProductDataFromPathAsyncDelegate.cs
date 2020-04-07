using System.Collections.Generic;

using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListApiProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataAsyncDelegate,GOG.Delegates",
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