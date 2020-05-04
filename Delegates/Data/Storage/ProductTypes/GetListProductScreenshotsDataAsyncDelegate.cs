using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.JSON.ProductTypes;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductScreenshotsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            typeof(GetStringDataAsyncDelegate),
            typeof(ConvertJSONToListProductScreenshotsDelegate))]
        public GetListProductScreenshotsDataAsyncDelegate(
            IGetDataAsyncDelegate<string, string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ProductScreenshots>> convertJSONToListProductScreenshotsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductScreenshotsDelegate)
        {
            // ...
        }
    }
}