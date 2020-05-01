using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListProductDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<Product>>
    {
        [Dependencies(
            typeof(GOG.Delegates.Data.Storage.ProductTypes.PostListProductDataAsyncDelegate),
            typeof(GetProductsPathDelegate))]
        public PostListProductDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<Product>> postListProductDataAsyncDelegate,
            IGetPathDelegate getProductPathDelegate) :
            base(
                postListProductDataAsyncDelegate,
                getProductPathDelegate)
        {
            // ...
        }
    }
}