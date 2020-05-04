using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using GOG.Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Data;
using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListAccountProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListAccountProductToJSONDelegate))]
        public PostListAccountProductDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate,
            IConvertDelegate<List<AccountProduct>, string> convertListAccountProductToJSONDelegate) :
            base(
                postStringDataAsyncDelegate,
                convertListAccountProductToJSONDelegate)
        {
            // ...
        }
    }
}