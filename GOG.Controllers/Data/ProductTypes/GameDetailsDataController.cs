using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;


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
            "GOG.Delegates.Find.ProductTypes.FindGameDetailsDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameDetailsDataController(
            IStashController<List<GameDetails>> gameDetailsStashController,
            IConvertDelegate<GameDetails, long> convertGameDetailsToIndexDelegate,
            IRecordsController<long> gameDetailsRecordsIndexController,
            IFindDelegate<GameDetails> findGameDetailsDelegate,
            IActionLogController actionLogController) :
            base(
                gameDetailsStashController,
                convertGameDetailsToIndexDelegate,
                gameDetailsRecordsIndexController,
                findGameDetailsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}