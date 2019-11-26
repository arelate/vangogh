using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.ValidationResults;

namespace Controllers.Data.ProductTypes
{
    public class ValidationResultsDataController : DataController<ValidationResults>
    {
        public ValidationResultsDataController(
            IStashController<List<ValidationResults>> validationResultsStashController,
            IConvertDelegate<ValidationResults, long> convertValidationResultsToIndexDelegate,
            IRecordsController<long> validationResultsRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                validationResultsStashController,
                convertValidationResultsToIndexDelegate,
                validationResultsRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}