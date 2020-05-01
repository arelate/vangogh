using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListUpdatedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(Delegates.Data.Storage.ProductTypes.GetListUpdatedDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate))]
        public GetListUpdatedDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getListUpdatedDataAsyncDelegate,
            IGetPathDelegate getUpdatedPathDelegate) :
            base(
                getListUpdatedDataAsyncDelegate,
                getUpdatedPathDelegate)
        {
            // ...
        }
    }
}