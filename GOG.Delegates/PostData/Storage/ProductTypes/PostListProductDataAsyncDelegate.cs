using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<Product>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertListProductToJSONDelegate,GOG.Delegates")]        
        public PostListProductDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<Product>, string> convertListProductToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListProductToJSONDelegate)
        {
            // ...
        }
    }
}