using System.Collections.Generic;
using Delegates.Data.Models;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;
using Delegates.Activities;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class CommitAccountProductsAsyncDelegate : CommitDataAsyncDelegate<AccountProduct>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.GetListAccountProductDataFromPathAsyncDelegate),
            typeof(GOG.Delegates.Data.Storage.ProductTypes.PostListAccountProductDataToPathAsyncDelegate),
            typeof(StartDelegate),
            typeof(CompleteDelegate))]
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