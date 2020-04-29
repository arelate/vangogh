using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class CommitGameDetailsAsyncDelegate : CommitDataAsyncDelegate<GameDetails>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameDetailsDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListGameDetailsDataToPathAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public CommitGameDetailsAsyncDelegate(
            IGetDataAsyncDelegate<List<GameDetails>, string> getDataAsyncDelegate,
            IPostDataAsyncDelegate<List<GameDetails>> postDataAsyncDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getDataAsyncDelegate,
                postDataAsyncDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}