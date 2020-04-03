using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListProductScreenshotsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetProductScreenshotsRecordsPathDelegate,Delegates")]
        public GetListProductScreenshotsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getProductScreenshotsRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getProductScreenshotsRecordsPathDelegate)
        {
            // ...
        }
    }
}