using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

using Delegates.PostData.Storage;

using GOG.Models;

namespace GOG.Delegates.PostData.Storage.ProductTypes
{
    public class PostListAccountProductDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListAccountProductDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetAccountProductsPathDelegate,Delegates")]
        public PostListAccountProductDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<AccountProduct>> postListAccountProductDataAsyncDelegate,
            IGetPathDelegate getAccountProductPathDelegate) :
            base(
                postListAccountProductDataAsyncDelegate,
                getAccountProductPathDelegate)
        {
            // ...
        }
    }
}