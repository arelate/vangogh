using System.Collections.Generic;

using Interfaces.Controllers.Records;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ValidationResultsDataController : DataController<ValidationResults>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListValidationResultsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListValidationResultsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertValidationResultsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ValidationResultsRecordsIndexController,Controllers",
            "Delegates.Collections.ProductTypes.FindValidationResultsDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public ValidationResultsDataController(
            IGetDataAsyncDelegate<List<ValidationResults>> getValidationResultsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ValidationResults>> postValidationResultsDataAsyncDelegate,
            IConvertDelegate<ValidationResults, long> convertValidationResultsToIndexDelegate,
            IRecordsController<long> validationResultsRecordsIndexController,
            IFindDelegate<ValidationResults> findValidationResultsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getValidationResultsDataAsyncDelegate,
                postValidationResultsDataAsyncDelegate,
                convertValidationResultsToIndexDelegate,
                validationResultsRecordsIndexController,
                findValidationResultsDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}