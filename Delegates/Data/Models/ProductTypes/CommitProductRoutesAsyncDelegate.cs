using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class CommitProductRoutesAsyncDelegate: CommitDataAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListProductRoutesDataToPathAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate")]
        public CommitProductRoutesAsyncDelegate(
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