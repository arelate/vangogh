using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListValidationResultsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListValidationResultsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetValidationResultsPathDelegate,Delegates")]
        public GetListValidationResultsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<long>> getListValidationResultsDataAsyncDelegate, 
            IGetPathDelegate getValidationResultsPathDelegate) : 
            base(
                getListValidationResultsDataAsyncDelegate, 
                getValidationResultsPathDelegate)
        {
            // ...
        }
    }
}