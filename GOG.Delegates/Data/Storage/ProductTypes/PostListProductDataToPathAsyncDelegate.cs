using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListProductDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<Product>>
    {
        [Dependencies(
            typeof(PostListProductDataAsyncDelegate),
            typeof(GetProductsPathDelegate))]
        public PostListProductDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<Product>> postListProductDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductPathDelegate) :
            base(
                postListProductDataAsyncDelegate,
                getProductPathDelegate)
        {
            // ...
        }
    }
}