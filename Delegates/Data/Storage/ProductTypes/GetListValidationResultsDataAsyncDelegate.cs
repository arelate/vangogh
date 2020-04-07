using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListValidationResultsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.Data.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ProductTypes.ConvertJSONToListValidationResultsDelegate,Delegates")]
        public GetListValidationResultsDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ValidationResults>> convertJSONToListValidationResultsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListValidationResultsDelegate)
        {
            // ...
        }
    }
}