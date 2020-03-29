using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.Records
{
    public class GetListValidationResultsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductRecordsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.Records.GetValidationResultsRecordsPathDelegate,Delegates")]
        public GetListValidationResultsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>> getListProductRecordsDataAsyncDelegate, 
            IGetPathDelegate getValidationResultsRecordsPathDelegate) : 
            base(
                getListProductRecordsDataAsyncDelegate, 
                getValidationResultsRecordsPathDelegate)
        {
            // ...
        }
    }
}