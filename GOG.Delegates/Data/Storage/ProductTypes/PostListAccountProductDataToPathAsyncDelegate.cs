using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
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
            IGetValueDelegate<string,(string Directory,string Filename)> getAccountProductPathDelegate) :
            base(
                postListAccountProductDataAsyncDelegate,
                getAccountProductPathDelegate)
        {
            // ...
        }
    }
}