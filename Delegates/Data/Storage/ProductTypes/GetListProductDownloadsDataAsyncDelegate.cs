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
            typeof(Delegates.Data.Storage.GetStringDataAsyncDelegate),
            typeof(Delegates.Convert.JSON.ProductTypes.ConvertJSONToListProductDownloadsDelegate))]
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