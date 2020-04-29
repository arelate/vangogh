using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;

namespace Delegates.Data.Models.ProductTypes
{
    public class CommitValidationResultsAsyncDelegate: CommitDataAsyncDelegate<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListValidationResultsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListValidationResultsDataToPathAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate")]
        public CommitValidationResultsAsyncDelegate(
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