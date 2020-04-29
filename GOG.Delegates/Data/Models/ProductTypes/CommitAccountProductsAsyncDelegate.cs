using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class CommitAccountProductsAsyncDelegate : CommitDataAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Data.Storage.ProductTypes.PostListAccountProductDataToPathAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public CommitAccountProductsAsyncDelegate(
            IGetDataAsyncDelegate<List<AccountProduct>, string> getDataAsyncDelegate,
            IPostDataAsyncDelegate<List<AccountProduct>> postDataAsyncDelegate,
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