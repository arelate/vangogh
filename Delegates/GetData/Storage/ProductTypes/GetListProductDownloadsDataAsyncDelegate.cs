using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListProductDownloadsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertJSONToListProductDownloadsDelegate,Delegates")]
        public GetListProductDownloadsDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ProductDownloads>> convertJSONToListProductDownloadsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductDownloadsDelegate)
        {
            // ...
        }
    }
}