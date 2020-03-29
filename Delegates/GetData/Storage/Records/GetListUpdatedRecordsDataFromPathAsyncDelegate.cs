using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListUpdatedRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetUpdatedRecordsPathDelegate,Delegates")]
        public GetListUpdatedRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getUpdatedRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getUpdatedRecordsPathDelegate)
        {
            // ...
        }
    }
}