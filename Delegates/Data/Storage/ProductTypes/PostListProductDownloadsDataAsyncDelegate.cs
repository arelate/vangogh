using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductDownloadsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.ProductTypes.ConvertListProductDownloadsToJSONDelegate))]
        public PostListProductDownloadsDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate,
            IConvertDelegate<List<ProductDownloads>, string> convertListProductDownloadsToJSONDelegate) :
            base(
                postStringDataAsyncDelegate,
                convertListProductDownloadsToJSONDelegate)
        {
            // ...
        }
    }
}