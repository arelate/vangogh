using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListGameProductDataRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetGameProductDataRecordsPathDelegate,Delegates")]
        public GetListGameProductDataRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getGameProductDataRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getGameProductDataRecordsPathDelegate)
        {
            // ...
        }
    }
}