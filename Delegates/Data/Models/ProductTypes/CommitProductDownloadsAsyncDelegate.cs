using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;
using Models.ProductTypes;

namespace Delegates.Data.Models.ProductTypes
{
    public class CommitProductDownloadsAsyncDelegate: CommitDataAsyncDelegate<ProductDownloads>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListProductDownloadsDataToPathAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate")]
        public CommitProductDownloadsAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductDownloads>, string> getDataAsyncDelegate, 
            IPostDataAsyncDelegate<List<ProductDownloads>> postDataAsyncDelegate, 
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