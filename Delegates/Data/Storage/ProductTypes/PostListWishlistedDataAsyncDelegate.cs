using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListWishlistedDataAsyncDelegate : PostJSONDataAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.Data.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertListLongToJSONDelegate,Delegates")]        
        public PostListWishlistedDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate, 
            IConvertDelegate<List<long>, string> convertListLongToJSONDelegate) : 
            base(
                postStringDataAsyncDelegate, 
                convertListLongToJSONDelegate)
        {
            // ...
        }
    }
}