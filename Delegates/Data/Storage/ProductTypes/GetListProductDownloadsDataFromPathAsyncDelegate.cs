using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductDownloadsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductDownloadsDataAsyncDelegate,Delegates",
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