using System.Collections.Generic;
using Attributes;
using Delegates.Values.Paths.ProductTypes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListUpdatedDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(PostListUpdatedDataAsyncDelegate),
            typeof(GetUpdatedPathDelegate))]
        public PostListUpdatedDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<long>> postListUpdatedDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getUpdatedPathDelegate) :
            base(
                postListUpdatedDataAsyncDelegate,
                getUpdatedPathDelegate)
        {
            // ...
        }
    }
}