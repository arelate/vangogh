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
            typeof(GetListProductScreenshotsDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetProductScreenshotsPathDelegate))]
        public GetListProductScreenshotsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductScreenshots>, string> getListProductScreenshotsDataAsyncDelegate,
            IGetPathDelegate getProductScreenshotsPathDelegate) :
            base(
                getListProductScreenshotsDataAsyncDelegate,
                getProductScreenshotsPathDelegate)
        {
            // ...
        }
    }
}