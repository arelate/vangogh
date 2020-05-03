using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductRoutesDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            typeof(GetListProductRoutesDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetProductRoutesPathDelegate))]
        public GetListProductRoutesDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductRoutes>, string> getListProductRoutesDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductRoutesPathDelegate) :
            base(
                getListProductRoutesDataAsyncDelegate,
                getProductRoutesPathDelegate)
        {
            // ...
        }
    }
}