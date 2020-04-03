using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListAccountProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertListAccountProductToJSONDelegate,GOG.Delegates")]        
        public PostListAccountProductDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<AccountProduct>, string> convertListAccountProductToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListAccountProductToJSONDelegate)
        {
            // ...
        }
    }
}