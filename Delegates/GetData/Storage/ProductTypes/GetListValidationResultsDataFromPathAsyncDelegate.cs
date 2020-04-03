using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListValidationResultsDataFromPathAsyncDelegate : 
        GetJSONDataFromPathAsyncDelegate<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListValidationResultsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetValidationResultsPathDelegate,Delegates")]
        public GetListValidationResultsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ValidationResults>> getListValidationResultsDataAsyncDelegate, 
            IGetPathDelegate getValidationResultsPathDelegate) : 
            base(
                getListValidationResultsDataAsyncDelegate, 
                getValidationResultsPathDelegate)
        {
            // ...
        }
    }
}