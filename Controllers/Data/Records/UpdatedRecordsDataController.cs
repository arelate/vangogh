using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class UpdatedRecordsDataController : DataController<ProductRecords>
    {
        public UpdatedRecordsDataController(
            IStashController<List<ProductRecords>> updatedRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                updatedRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}