using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Models.ProductTypes;

namespace Delegates.PostData.Storage.ProductTypes
{
    public class PostListValidationResultsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertListValidationResultsToJSONDelegate,Delegates")]        
        public PostListValidationResultsDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<ValidationResults>, string> convertListValidationResultsToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListValidationResultsToJSONDelegate)
        {
            // ...
        }
    }
}