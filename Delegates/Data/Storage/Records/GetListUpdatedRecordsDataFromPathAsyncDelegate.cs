using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class GetListUpdatedRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.Records.GetListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetUpdatedRecordsPathDelegate))]
        public GetListUpdatedRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>, string> getListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getUpdatedRecordsPathDelegate) :
            base(
                getListProductRecordsDataAsyncDelegate,
                getUpdatedRecordsPathDelegate)
        {
            // ...
        }
    }
}