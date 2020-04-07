using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductScreenshotsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.Data.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ProductTypes.ConvertListProductScreenshotsToJSONDelegate,Delegates")]        
        public PostListProductScreenshotsDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<ProductScreenshots>, string> convertListProductScreenshotsToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListProductScreenshotsToJSONDelegate)
        {
            // ...
        }
    }
}