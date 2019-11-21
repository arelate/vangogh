using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.ValidationResults;

namespace Controllers.Data.ProductTypes
{
    public class ValidationResultsDataController : DataController<ValidationResults>
    {
        public ValidationResultsDataController(
            IStashController<Dictionary<long, ValidationResults>> validationResultsStashController,
            IConvertDelegate<ValidationResults, long> convertValidationResultsToIndexDelegate,
            IRecordsController<long> validationResultsRecordsIndexController,
            IStatusController statusController,
            ICommitAsyncDelegate hashesController) :
            base(
                validationResultsStashController,
                convertValidationResultsToIndexDelegate,
                validationResultsRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}