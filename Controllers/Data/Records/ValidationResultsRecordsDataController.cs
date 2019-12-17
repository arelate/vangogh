using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class ValidationResultsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Controllers.Stash.Records.ValidationResultsRecordsStashController,Controllers",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ValidationResultsRecordsDataController(
            IStashController<List<ProductRecords>> validationResultsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                validationResultsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}