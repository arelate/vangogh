using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using GOG.Models;
using Delegates.Data.Storage;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<Product>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertListProductToJSONDelegate))]
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