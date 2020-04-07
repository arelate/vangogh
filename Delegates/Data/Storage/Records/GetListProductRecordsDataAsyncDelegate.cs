using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class GetListProductRecordsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.Data.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.Records.ConvertJSONToListProductRecordsDelegate,Delegates")]
        public GetListProductRecordsDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ProductRecords>> convertJSONToListProductRecordsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductRecordsDelegate)
        {
            // ...
        }
    }
}