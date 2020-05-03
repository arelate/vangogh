using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        GetListApiProductsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(GetListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetApiProductsRecordsPathDelegate))]
        public GetListApiProductsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>, string> getListProductRecordsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getApiProductsRecordsPathDelegate) :
            base(
                getListProductRecordsDataAsyncDelegate,
                getApiProductsRecordsPathDelegate)
        {
            // ...
        }
    }
}