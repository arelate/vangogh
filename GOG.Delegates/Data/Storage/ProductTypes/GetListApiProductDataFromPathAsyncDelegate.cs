using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class GetListApiProductDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataAsyncDelegate),
            typeof(GetApiProductsPathDelegate))]
        public GetListApiProductDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ApiProduct>,string> getListApiProductDataAsyncDelegate,
            IGetPathDelegate getApiProductsPathDelegate) :
            base(
                getListApiProductDataAsyncDelegate,
                getApiProductsPathDelegate)
        {
            // ...
        }
    }
}