using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListUpdatedDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(PostListUpdatedDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate))]
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