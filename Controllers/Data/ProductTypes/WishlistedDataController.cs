using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class WishlistedDataController : DataController<long>
    {
        [Dependencies(
            "Controllers.Stash.ProductTypes.WishlistedStashController,Controllers",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.WishlistedRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
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