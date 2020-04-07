using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class GetListProductScreenshotsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.Data.Storage.Records.GetListProductRecordsDataAsyncDelegate,Delegates",
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