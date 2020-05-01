using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Activities;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class CommitGameProductDataAsyncDelegate : CommitDataAsyncDelegate<GameProductData>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListGameProductDataDataFromPathAsyncDelegate),
            typeof(GOG.Delegates.Data.Storage.ProductTypes.PostListGameProductDataDataToPathAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
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