using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Models.Dependencies;

using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class WishlistedDataController : DataController<long>
    {
        [Dependencies(
            DependencyContext.Default,
            "Controllers.Stash.ProductTypes.WishlistedStashController,Controllers",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.WishlistedRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public WishlistedDataController(
            IStashController<List<long>> wishlistedDataController,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> wishlistedRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                wishlistedDataController,
                convertPassthroughIndexDelegate,
                wishlistedRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}