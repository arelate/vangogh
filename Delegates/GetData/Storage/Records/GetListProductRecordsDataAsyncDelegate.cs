using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListProductRecordsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertJSONToListProductRecordsDelegate,Delegates")]
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