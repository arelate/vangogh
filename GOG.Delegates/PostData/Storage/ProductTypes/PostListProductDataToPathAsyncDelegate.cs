using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListProductDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<Product>>
    {
        [Dependencies(
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListProductDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetProductsPathDelegate,Delegates")]
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