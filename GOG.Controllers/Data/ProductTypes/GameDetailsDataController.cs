using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameDetailsDataController : DataController<GameDetails>
    {
        [Dependencies(
            "GOG.Controllers.Stash.ProductTypes.GameDetailsStashController,GOG.Controllers",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.GameDetailsRecordsIndexController,Controllers",
            "Controllers.Collection.CollectionController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameDetailsDataController(
            IStashController<List<GameDetails>> gameDetailsStashController,
            IConvertDelegate<GameDetails, long> convertGameDetailsToIndexDelegate,
            IRecordsController<long> gameDetailsRecordsIndexController,
            ICollectionController collectionController,
            IActionLogController actionLogController) :
            base(
                gameDetailsStashController,
                convertGameDetailsToIndexDelegate,
                gameDetailsRecordsIndexController,
                collectionController,
                actionLogController)
        {
            // ...
        }
    }
}