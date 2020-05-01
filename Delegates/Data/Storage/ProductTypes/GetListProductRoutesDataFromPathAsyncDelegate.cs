using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductRoutesDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListProductRoutesDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetProductRoutesPathDelegate))]
        public GetListProductRoutesDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRoutes>, string> getListProductRoutesDataAsyncDelegate,
            IGetPathDelegate getProductRoutesPathDelegate) :
            base(
                getListProductRoutesDataAsyncDelegate,
                getProductRoutesPathDelegate)
        {
            // ...
        }
    }
}