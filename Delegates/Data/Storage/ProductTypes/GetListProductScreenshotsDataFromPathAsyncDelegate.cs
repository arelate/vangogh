using System.Collections.Generic;
using Attributes;
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
            typeof(Delegates.GetPath.ProductTypes.GetProductScreenshotsPathDelegate))]
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