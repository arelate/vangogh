using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListGameProductDataDataAsyncDelegate : PostJSONDataAsyncDelegate<List<GameProductData>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListGameProductDataToJSONDelegate))]
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