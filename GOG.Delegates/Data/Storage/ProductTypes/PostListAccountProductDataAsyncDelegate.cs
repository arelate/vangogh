using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListAccountProductDataAsyncDelegate : PostJSONDataAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertListAccountProductToJSONDelegate))]
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