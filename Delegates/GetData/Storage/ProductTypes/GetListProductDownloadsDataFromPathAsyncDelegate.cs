using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListProductDownloadsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductDownloadsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetProductDownloadsPathDelegate,Delegates")]
        public GetListProductDownloadsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductDownloads>> getListProductDownloadsDataAsyncDelegate, 
            IGetPathDelegate getProductDownloadsPathDelegate) : 
            base(
                getListProductDownloadsDataAsyncDelegate, 
                getProductDownloadsPathDelegate)
        {
            // ...
        }
    }
}