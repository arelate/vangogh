using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        GetListGameDetailsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.Records.GetListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetGameDetailsRecordsPathDelegate))]
        public GetListGameDetailsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>, string> getListProductRecordsDataAsyncDelegate,
            IGetPathDelegate getGameDetailsRecordsPathDelegate) :
            base(
                getListProductRecordsDataAsyncDelegate,
                getGameDetailsRecordsPathDelegate)
        {
            // ...
        }
    }
}