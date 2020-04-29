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
            "Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListProductRoutesDataToPathAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate")]
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