using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListWishlistedDataAsyncDelegate : PostJSONDataAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.System.ConvertListLongToJSONDelegate))]
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