using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.Records;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class GetListProductRecordsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(ConvertJSONToListProductRecordsDelegate))]
        public GetListProductRecordsDataAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ProductRecords>> convertJSONToListProductRecordsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductRecordsDelegate)
        {
            // ...
        }
    }
}