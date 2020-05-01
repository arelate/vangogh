using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductRoutesDataAsyncDelegate : PostJSONDataAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.PostStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.ProductTypes.ConvertListProductRoutesToJSONDelegate))]
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