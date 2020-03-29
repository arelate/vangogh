using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListProductDownloadsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetProductDownloadsRecordsPathDelegate,Delegates")]
        public GetListProductDownloadsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getProductDownloadsRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getProductDownloadsRecordsPathDelegate)
        {
            // ...
        }
    }
}