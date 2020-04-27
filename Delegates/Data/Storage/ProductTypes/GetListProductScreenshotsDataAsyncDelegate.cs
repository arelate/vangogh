using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Data;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class GetListProductScreenshotsDataAsyncDelegate : GetJSONDataAsyncDelegate<List<ProductScreenshots>>
    {
        [Dependencies(
            "Delegates.Data.Storage.GetStringDataAsyncDelegate,Delegates",
            "Delegates.Convert.JSON.ProductTypes.ConvertJSONToListProductScreenshotsDelegate,Delegates")]
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