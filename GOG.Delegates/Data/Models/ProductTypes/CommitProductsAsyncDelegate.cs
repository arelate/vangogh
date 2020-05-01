using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Activities;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class CommitProductsAsyncDelegate : CommitDataAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate),
            typeof(GOG.Delegates.Data.Storage.ProductTypes.PostListApiProductDataToPathAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
        public CommitProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<ApiProduct>, string> getDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ApiProduct>> postDataAsyncDelegate,
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