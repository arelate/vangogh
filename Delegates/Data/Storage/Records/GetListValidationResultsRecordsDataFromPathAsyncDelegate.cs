using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.Records
{
    public class
        GetListValidationResultsRecordsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRecords>
        >
    {
        [Dependencies(
            typeof(GetListProductRecordsDataAsyncDelegate),
            typeof(Delegates.GetPath.Records.GetValidationResultsRecordsPathDelegate))]
        public GetListValidationResultsRecordsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRecords>, string> getListProductRecordsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getValidationResultsRecordsPathDelegate) :
            base(
                getListProductRecordsDataAsyncDelegate,
                getValidationResultsRecordsPathDelegate)
        {
            // ...
        }
    }
}