using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

namespace Controllers.Data.ProductTypes
{
    public class UpdatedDataController : DataController<long>
    {
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