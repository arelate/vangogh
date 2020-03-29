using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListProductScreenshotsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductScreenshotsDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetProductScreenshotsPathDelegate,Delegates")]
        public GetListProductScreenshotsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductScreenshots>> getListProductScreenshotsDataAsyncDelegate, 
            IGetPathDelegate getProductScreenshotsPathDelegate) : 
            base(
                getListProductScreenshotsDataAsyncDelegate, 
                getProductScreenshotsPathDelegate)
        {
            // ...
        }
    }
}