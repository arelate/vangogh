using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductDownloadsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            "Delegates.Data.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ProductTypes.ConvertJSONToListProductDownloadsDelegate,Delegates")]
        public GetListProductDownloadsDataAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ProductDownloads>> convertJSONToListProductDownloadsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductDownloadsDelegate)
        {
            // ...
        }
    }
}