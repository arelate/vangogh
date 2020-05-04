using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductDownloadsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(ConvertJSONToListProductDownloadsDelegate))]
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