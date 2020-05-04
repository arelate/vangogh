using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListApiProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListApiProductToJSONDelegate))]
        public PostListApiProductDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate,
            IConvertDelegate<List<ApiProduct>, string> convertListApiProductToJSONDelegate) :
            base(
                postStringDataAsyncDelegate,
                convertListApiProductToJSONDelegate)
        {
            // ...
        }
    }
}