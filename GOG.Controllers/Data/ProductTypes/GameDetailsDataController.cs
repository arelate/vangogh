using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameDetailsDataController : DataController<GameDetails>
    {
        public GameDetailsDataController(
            IStashController<List<GameDetails>> gameDetailsStashController,
            IConvertDelegate<GameDetails, long> convertGameDetailsToIndexDelegate,
            IRecordsController<long> gameDetailsRecordsIndexController,
            ICollectionController collectionController,
            IStatusController statusController) :
            base(
                gameDetailsStashController,
                convertGameDetailsToIndexDelegate,
                gameDetailsRecordsIndexController,
                collectionController,
                statusController)
        {
            // ...
        }
    }
}