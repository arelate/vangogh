using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.PostData;
using Interfaces.Delegates.GetPath;

namespace Delegates.PostData.Storage.ProductTypes
{
    public class PostUpdatedDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.PostData.Storage.ProductTypes.PostUpdatedDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate,Delegates")]
        public PostUpdatedDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<long>> postUpdatedDataAsyncDelegate,
            IGetPathDelegate getPathDelegate) :
            base(
                postUpdatedDataAsyncDelegate,
                getPathDelegate)
        {
            // ...
        }
    }
}