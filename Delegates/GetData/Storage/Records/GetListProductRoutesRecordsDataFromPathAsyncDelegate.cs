using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListProductRoutesRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetProductRoutesRecordsPathDelegate,Delegates")]
        public GetListProductRoutesRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getProductRoutesRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getProductRoutesRecordsPathDelegate)
        {
            // ...
        }
    }
}