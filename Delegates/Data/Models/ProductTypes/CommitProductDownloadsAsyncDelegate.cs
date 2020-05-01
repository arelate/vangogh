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
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataFromPathAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.PostListProductDownloadsDataToPathAsyncDelegate),
            typeof(Activities.StartDelegate),
            typeof(Activities.CompleteDelegate))]
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