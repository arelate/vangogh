using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListGameDetailsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetGameDetailsRecordsPathDelegate,Delegates")]
        public GetListGameDetailsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getGameDetailsRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getGameDetailsRecordsPathDelegate)
        {
            // ...
        }
    }
}