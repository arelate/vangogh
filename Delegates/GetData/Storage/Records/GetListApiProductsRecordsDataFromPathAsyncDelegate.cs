using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListApiProductsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetApiProductsRecordsPathDelegate,Delegates")]
        public GetListApiProductsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getApiProductsRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getApiProductsRecordsPathDelegate)
        {
            // ...
        }
    }
}