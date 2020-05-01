using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListGameProductDataDataAsyncDelegate : PostJSONDataAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertListGameProductDataToJSONDelegate))]
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