using System.Collections.Generic;

using Attributes;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetData;

using Models.ProductTypes;

namespace Delegates.GetData.Storage.ProductTypes
{
    public class GetListProductScreenshotsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.GetData.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.System.ConvertJSONToListProductScreenshotsDelegate,Delegates")]
        public GetListProductScreenshotsDataAsyncDelegate(
            IGetDataAsyncDelegate<string> getStringDataAsyncDelegate,
            IConvertDelegate<string, List<ProductScreenshots>> convertJSONToListProductScreenshotsDelegate) :
            base(
                getStringDataAsyncDelegate,
                convertJSONToListProductScreenshotsDelegate)
        {
            // ...
        }
    }
}