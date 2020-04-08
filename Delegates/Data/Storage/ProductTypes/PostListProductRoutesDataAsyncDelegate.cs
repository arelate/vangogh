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
            "Delegates.Data.Storage.PostStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ProductTypes.ConvertListProductRoutesToJSONDelegate,Delegates")]
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