using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListUpdatedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            typeof(GetListUpdatedDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate))]
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