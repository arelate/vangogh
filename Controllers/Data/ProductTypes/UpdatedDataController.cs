using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class UpdatedDataController : DataController<long>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.UpdatedStashController,Controllers",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.UpdatedRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public UpdatedDataController(
            IStashController<List<long>> updatedDataController,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> updatedRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                updatedDataController,
                convertPassthroughIndexDelegate,
                updatedRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}