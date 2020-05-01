using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListApiProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertListApiProductToJSONDelegate))]
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