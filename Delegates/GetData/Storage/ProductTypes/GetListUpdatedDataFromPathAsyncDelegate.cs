using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListUpdatedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetUpdatedDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate,Delegates")]
        public GetListUpdatedDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<long>> getListUpdatedDataAsyncDelegate, 
            IGetPathDelegate getUpdatedPathDelegate) : 
            base(
                getListUpdatedDataAsyncDelegate, 
                getUpdatedPathDelegate)
        {
            // ...
        }
    }
}