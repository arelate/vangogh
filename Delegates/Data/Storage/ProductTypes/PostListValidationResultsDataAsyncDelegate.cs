using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListValidationResultsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ValidationResults>>
    {
        [Dependencies(
            "Delegates.Data.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ProductTypes.ConvertListValidationResultsToJSONDelegate,Delegates")]        
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