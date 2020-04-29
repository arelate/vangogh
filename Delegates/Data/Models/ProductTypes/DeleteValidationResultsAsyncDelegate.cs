using System.Collections.Generic;
using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class DeleteValidationResultsAsyncDelegate : DeleteAsyncDelegate<ValidationResults>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListValidationResultsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertValidationResultsToIndexDelegate,Delegates",
            "Delegates.Confirm.ProductTypes.ConfirmValidationResultsContainIdAsyncDelegate,Delegates")]
        public DeleteValidationResultsAsyncDelegate(
            IGetDataAsyncDelegate<List<ValidationResults>, string> getDataCollectionAsyncDelegate,
            IConvertDelegate<ValidationResults, long> convertProductToIndexDelegate,
            IConfirmAsyncDelegate<long> confirmContainsAsyncDelegate) :
            base(
                getDataCollectionAsyncDelegate,
                convertProductToIndexDelegate,
                confirmContainsAsyncDelegate)
        {
            // ...
        }
    }
}