using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductRoutesDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.PostListProductRoutesDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetProductRoutesPathDelegate))]
        public PostListProductRoutesDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRoutes>> postListProductRoutesDataAsyncDelegate,
            IGetPathDelegate getListProductRoutesPathDelegate) :
            base(
                postListProductRoutesDataAsyncDelegate,
                getListProductRoutesPathDelegate)
        {
            // ...
        }
    }
}