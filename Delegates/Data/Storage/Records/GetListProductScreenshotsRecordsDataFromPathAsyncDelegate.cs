using System.Collections.Generic;
using Attributes;
using Delegates.Values.Paths.Records;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        GetListProductScreenshotsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<
            List<ProductRecords>>
    {
        [Dependencies(
            typeof(GetListProductRecordsDataAsyncDelegate),
            typeof(GetProductScreenshotsRecordsPathDelegate))]
        public GetListProductScreenshotsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>, string> getListProductRecordsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductScreenshotsRecordsPathDelegate) :
            base(
                getListProductRecordsDataAsyncDelegate,
                getProductScreenshotsRecordsPathDelegate)
        {
            // ...
        }
    }
}