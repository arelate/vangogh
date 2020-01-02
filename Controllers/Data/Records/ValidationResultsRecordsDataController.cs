using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

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
            "Controllers.Logs.ActionLogController,Controllers")]
        public ValidationResultsRecordsDataController(
            IStashController<List<ProductRecords>> validationResultsRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                validationResultsRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}