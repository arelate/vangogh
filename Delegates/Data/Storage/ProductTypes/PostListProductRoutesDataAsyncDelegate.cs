using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductRoutesDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            typeof(PostStringDataAsyncDelegate),
            typeof(ConvertListProductRoutesToJSONDelegate))]
        public PostListProductRoutesDataAsyncDelegate(
            IPostDataAsyncDelegate<string> postStringDataAsyncDelegate,
            IConvertDelegate<List<ProductRoutes>, string> convertListProductRoutesToJSONDelegate) :
            base(
                postStringDataAsyncDelegate,
                convertListProductRoutesToJSONDelegate)
        {
            // ...
        }
    }
}