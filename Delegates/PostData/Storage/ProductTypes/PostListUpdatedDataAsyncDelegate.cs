using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.PostData;

namespace Delegates.PostData.Storage.ProductTypes
{
    public class PostListUpdatedDataAsyncDelegate : PostJSONDataAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertListLongToJSONDelegate,Delegates")]        
        public PostListUpdatedDataAsyncDelegate(
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