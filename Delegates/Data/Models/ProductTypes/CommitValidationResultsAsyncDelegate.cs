using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Data;
using Attributes;
using Models.ProductTypes;

namespace Delegates.Data.Models.ProductTypes
{
    public class CommitValidationResultsAsyncDelegate: CommitDataAsyncDelegate<ValidationResults>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListValidationResultsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListValidationResultsDataToPathAsyncDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate")]
        public CommitValidationResultsAsyncDelegate(
            IGetDataAsyncDelegate<List<ValidationResults>, string> getDataAsyncDelegate, 
            IPostDataAsyncDelegate<List<ValidationResults>> postDataAsyncDelegate, 
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