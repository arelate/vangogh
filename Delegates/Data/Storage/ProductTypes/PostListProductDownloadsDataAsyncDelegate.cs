using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductDownloadsDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListProductDownloadsToJSONDelegate))]
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