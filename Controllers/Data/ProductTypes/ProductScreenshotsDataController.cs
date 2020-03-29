using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ProductScreenshotsDataController : DataController<ProductScreenshots>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.ProductTypes.PostListProductScreenshotsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductScreenshotsRecordsIndexController,Controllers",
            "Delegates.Find.ProductTypes.FindProductScreenshotsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductScreenshotsDataController(
            IGetDataAsyncDelegate<List<ProductScreenshots>> getProductScreenshotsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductScreenshots>> postProductScreenshotsDataAsyncDelegate,
            IConvertDelegate<ProductScreenshots, long> convertProductScreenshotsToIndexDelegate,
            IRecordsController<long> productScreenshotsRecordsIndexController,
            IFindDelegate<ProductScreenshots> findProductScreenshotsDelegate,
            IActionLogController actionLogController) :
            base(
                getProductScreenshotsDataAsyncDelegate,
                postProductScreenshotsDataAsyncDelegate,
                convertProductScreenshotsToIndexDelegate,
                productScreenshotsRecordsIndexController,
                findProductScreenshotsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}