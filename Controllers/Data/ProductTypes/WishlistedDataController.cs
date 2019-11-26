using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

namespace Controllers.Data.ProductTypes
{
    public class WishlistedDataController : DataController<long>
    {
        public WishlistedDataController(
            IStashController<List<long>> wishlistedDataController,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> wishlistedRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                wishlistedDataController,
                convertPassthroughIndexDelegate,
                wishlistedRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}