using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        GetListApiProductsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.Records.GetListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetApiProductsRecordsPathDelegate))]
        public GetListApiProductsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>, string> getListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getApiProductsRecordsPathDelegate) :
            base(
                getListProductRecordsDataAsyncDelegate,
                getApiProductsRecordsPathDelegate)
        {
            // ...
        }
    }
}