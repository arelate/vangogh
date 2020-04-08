using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class
        GetListProductScreenshotsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataAsyncDelegate,Delegates",
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