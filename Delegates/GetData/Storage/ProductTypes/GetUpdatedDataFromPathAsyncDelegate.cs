using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.GetPath;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetUpdatedDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<long>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetUpdatedDataAsyncDelegate,Delegates",
            "Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate,Delegates")]
        public GetUpdatedDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<long>> getUpdatedDataAsyncDelegate, 
            IGetPathDelegate getUpdatedPathDelegate) : 
            base(
                getUpdatedDataAsyncDelegate, 
                getUpdatedPathDelegate)
        {
            // ...
        }
    }
}