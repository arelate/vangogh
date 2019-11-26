using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Data.Records
{
    public class WishlistedRecordsDataController : DataController<ProductRecords>
    {
        public WishlistedRecordsDataController(
            IStashController<List<ProductRecords>> wishlistedRecordsStashController,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                wishlistedRecordsStashController,
                convertProductRecordsToIndexDelegate,
                null,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}