using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListAccountProductDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<AccountProduct>>
    {
        [Dependencies(
            typeof(PostListAccountProductDataAsyncDelegate),
            typeof(GetAccountProductsPathDelegate))]
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