using System.Collections.Generic;
using Interfaces.Controllers.Records;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;
using Controllers.Data;
using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameDetailsDataController : DataController<GameDetails>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListGameDetailsDataToPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.GameDetailsRecordsIndexController,Controllers",
            "GOG.Delegates.Collections.ProductTypes.FindGameDetailsDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public GameDetailsDataController(
            IGetDataAsyncDelegate<List<GameDetails>> getListGameDetailsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<GameDetails>> postListGameDetailsDataAsyncDelegate,
            IConvertDelegate<GameDetails, long> convertGameDetailsToIndexDelegate,
            IRecordsController<long> gameDetailsRecordsIndexController,
            IFindDelegate<GameDetails> findGameDetailsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getListGameDetailsDataAsyncDelegate,
                postListGameDetailsDataAsyncDelegate,
                convertGameDetailsToIndexDelegate,
                gameDetailsRecordsIndexController,
                findGameDetailsDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}