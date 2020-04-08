using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListValidationResultsDataFromPathAsyncDelegate :
        GetJSONDataFromPathAsyncDelegate<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListValidationResultsDataAsyncDelegate,Delegates",
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