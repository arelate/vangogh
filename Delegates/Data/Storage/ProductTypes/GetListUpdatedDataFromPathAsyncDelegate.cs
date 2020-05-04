using System.Collections.Generic;
using Attributes;
using Delegates.Values.Paths.ProductTypes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListUpdatedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(GetListUpdatedDataAsyncDelegate),
            typeof(GetUpdatedPathDelegate))]
        public GetListUpdatedDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<long>, string> getListUpdatedDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getUpdatedPathDelegate) :
            base(
                getListUpdatedDataAsyncDelegate,
                getUpdatedPathDelegate)
        {
            // ...
        }
    }
}