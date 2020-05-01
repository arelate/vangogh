using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class CommitProductsAsyncDelegate : CommitDataAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListApiProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListApiProductDataToPathAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
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