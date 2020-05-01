using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class CommitWishlistedAsyncDelegate: CommitDataAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.PostListWishlistedDataToPathAsyncDelegate),
            typeof(Activities.StartDelegate),
            typeof(Activities.CompleteDelegate))]
        public CommitWishlistedAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getDataAsyncDelegate, 
            IPostDataAsyncDelegate<List<long>> postDataAsyncDelegate, 
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