using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class CommitGameProductDataAsyncDelegate : CommitDataAsyncDelegate<GameProductData>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListGameProductDataDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListGameProductDataDataToPathAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public CommitGameProductDataAsyncDelegate(
            IGetDataAsyncDelegate<List<GameProductData>, string> getDataAsyncDelegate,
            IPostDataAsyncDelegate<List<GameProductData>> postDataAsyncDelegate,
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