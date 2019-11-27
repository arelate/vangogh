using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.ValidationResults;

namespace Controllers.Data.ProductTypes
{
    public class ValidationResultsDataController : DataController<ValidationResults>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.ValidationResultsStashController,Controllers",
            "Delegates.Convert.ProductTypes.ConvertValidationResultsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ValidationResultsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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