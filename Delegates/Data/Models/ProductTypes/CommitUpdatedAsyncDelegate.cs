using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class CommitUpdatedAsyncDelegate: CommitDataAsyncDelegate<long>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListUpdatedDataFromPathAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.PostListUpdatedDataToPathAsyncDelegate),
            typeof(Activities.StartDelegate),
            typeof(Activities.CompleteDelegate))]
        public CommitUpdatedAsyncDelegate(
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