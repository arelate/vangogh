using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class UpdateValidationResultsAsyncDelegate: UpdateDataAsyncDelegate<ValidationResults>
    {
        [Dependencies(
            "Delegates.Data.Models.ProductTypes.DeleteValidationResultsAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertValidationResultsToIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmValidationResultsContainIdAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.GetListValidationResultsDataFromPathAsyncDelegate,Delegates")]
        public UpdateValidationResultsAsyncDelegate(
            IDeleteAsyncDelegate<ValidationResults> deleteAsyncDelegate, 
            IConvertDelegate<ValidationResults, long> convertProductToIndexDelegate, 
            IConfirmAsyncDelegate<long> confirmDataContainsIdAsyncDelegate, 
            IGetDataAsyncDelegate<List<ValidationResults>, string> getListValidationResultsAsyncDelegate) : 
            base(
                deleteAsyncDelegate,
                convertProductToIndexDelegate, 
                confirmDataContainsIdAsyncDelegate, 
                getListValidationResultsAsyncDelegate)
        {
            // ...
        }
    }
}