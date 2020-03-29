using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ValidationResultsDataController : DataController<ValidationResults>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListValidationResultsDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.ProductTypes.PostListValidationResultsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertValidationResultsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ValidationResultsRecordsIndexController,Controllers",
            "Delegates.Find.ProductTypes.FindValidationResultsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ValidationResultsDataController(
            IGetDataAsyncDelegate<List<ValidationResults>> getValidationResultsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ValidationResults>> postValidationResultsDataAsyncDelegate,
            IConvertDelegate<ValidationResults, long> convertValidationResultsToIndexDelegate,
            IRecordsController<long> validationResultsRecordsIndexController,
            IFindDelegate<ValidationResults> findValidationResultsDelegate,
            IActionLogController actionLogController) :
            base(
                getValidationResultsDataAsyncDelegate,
                postValidationResultsDataAsyncDelegate,
                convertValidationResultsToIndexDelegate,
                validationResultsRecordsIndexController,
                findValidationResultsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}