using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class CommitApiProductsAsyncDelegate : CommitDataAsyncDelegate<ApiProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Models.ProductTypes.DeleteApiProductsAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListApiProductDataToPathAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public CommitApiProductsAsyncDelegate(
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