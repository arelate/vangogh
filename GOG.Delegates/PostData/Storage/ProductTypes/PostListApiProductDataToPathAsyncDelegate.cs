using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListApiProductDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ApiProduct>>
    {
        [Dependencies(
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListApiProductDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetApiProductsPathDelegate,Delegates")]
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