using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.System;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListWishlistedDataAsyncDelegate : PostJSONDataAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListLongToJSONDelegate))]
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