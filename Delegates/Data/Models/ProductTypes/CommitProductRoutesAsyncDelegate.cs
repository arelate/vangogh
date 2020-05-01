using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;
using Models.ProductTypes;

namespace Delegates.Data.Models.ProductTypes
{
    public class CommitProductRoutesAsyncDelegate: CommitDataAsyncDelegate<ProductRoutes>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate),
            typeof(Delegates.Data.Storage.ProductTypes.PostListProductRoutesDataToPathAsyncDelegate),
            typeof(Activities.StartDelegate),
            typeof(Activities.CompleteDelegate))]
        public CommitProductRoutesAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRoutes>, string> getDataAsyncDelegate, 
            IPostDataAsyncDelegate<List<ProductRoutes>> postDataAsyncDelegate, 
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