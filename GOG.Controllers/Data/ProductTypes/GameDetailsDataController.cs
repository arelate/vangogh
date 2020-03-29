using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameDetailsDataController : DataController<GameDetails>
    {
        [Dependencies(
            "GOG.Delegates.GetData.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListGameDetailsDataToPathAsyncDelegate,GOG.Delegates",            
            "GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.GameDetailsRecordsIndexController,Controllers",
            "GOG.Delegates.Find.ProductTypes.FindGameDetailsDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameDetailsDataController(
            IGetDataAsyncDelegate<List<GameDetails>> getListGameDetailsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<GameDetails>> postListGameDetailsDataAsyncDelegate,
            IConvertDelegate<GameDetails, long> convertGameDetailsToIndexDelegate,
            IRecordsController<long> gameDetailsRecordsIndexController,
            IFindDelegate<GameDetails> findGameDetailsDelegate,
            IActionLogController actionLogController) :
            base(
                getListGameDetailsDataAsyncDelegate,
                postListGameDetailsDataAsyncDelegate,
                convertGameDetailsToIndexDelegate,
                gameDetailsRecordsIndexController,
                findGameDetailsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}