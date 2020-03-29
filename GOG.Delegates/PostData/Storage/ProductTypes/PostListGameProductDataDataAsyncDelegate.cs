using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListGameProductDataDataAsyncDelegate : PostJSONDataAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertListGameProductDataToJSONDelegate,Delegates")]        
        public PostListGameProductDataDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<GameProductData>, string> convertListGameProductDataToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListGameProductDataToJSONDelegate)
        {
            // ...
        }
    }
}