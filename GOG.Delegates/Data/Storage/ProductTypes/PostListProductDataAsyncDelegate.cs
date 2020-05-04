using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<Product>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListProductToJSONDelegate))]
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