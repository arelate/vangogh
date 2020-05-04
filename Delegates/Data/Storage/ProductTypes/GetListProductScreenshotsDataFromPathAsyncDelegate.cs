using System.Collections.Generic;
using Attributes;
using Delegates.Values.Paths.ProductTypes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class
        GetListProductScreenshotsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            typeof(GetListProductScreenshotsDataAsyncDelegate),
            typeof(GetProductScreenshotsPathDelegate))]
        public GetListProductScreenshotsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductScreenshots>, string> getListProductScreenshotsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductScreenshotsPathDelegate) :
            base(
                getListProductScreenshotsDataAsyncDelegate,
                getProductScreenshotsPathDelegate)
        {
            // ...
        }
    }
}