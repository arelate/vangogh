using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class UpdatedRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Controllers.Stash.Records.UpdatedRecordsStashController,Controllers",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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