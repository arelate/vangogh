using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListApiProductDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            typeof(PostListApiProductDataAsyncDelegate),
            typeof(GetApiProductsPathDelegate))]
        public PostListApiProductDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ApiProduct>> postListApiProductDataAsyncDelegate,
            IGetPathDelegate getApiProductPathDelegate) :
            base(
                postListApiProductDataAsyncDelegate,
                getApiProductPathDelegate)
        {
            // ...
        }
    }
}