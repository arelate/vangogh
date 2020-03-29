using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListValidationResultsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertJSONToListValidationResultsDelegate,Delegates")]
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