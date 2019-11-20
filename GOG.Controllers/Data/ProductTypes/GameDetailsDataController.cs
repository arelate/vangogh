using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Data;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameDetailsDataController : DataController<GameDetails>
    {
        public GameDetailsDataController(
            IStashController<Dictionary<long, GameDetails>> gameDetailsStashController,
            IConvertDelegate<GameDetails, long> convertGameDetailsToIndexDelegate,
            IRecordsController<long> gameDetailsRecordsIndexController,
            IStatusController statusController,
            params ICommitAsyncDelegate[] hashesController) :
            base(
                gameDetailsStashController,
                convertGameDetailsToIndexDelegate,
                gameDetailsRecordsIndexController,
                statusController,
                hashesController)
        {
            // ...
        }
    }
}