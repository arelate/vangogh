using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class CommitProductDownloadsAsyncDelegate: CommitDataAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListProductDownloadsDataToPathAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate")]
        public CommitProductDownloadsAsyncDelegate(
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