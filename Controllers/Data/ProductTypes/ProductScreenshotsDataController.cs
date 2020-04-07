using System.Collections.Generic;

using Interfaces.Controllers.Records;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.ProductTypes
{
    public class ProductScreenshotsDataController : DataController<ProductScreenshots>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListProductScreenshotsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListProductScreenshotsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ProductTypes.ConvertProductScreenshotsToIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.ProductScreenshotsRecordsIndexController,Controllers",
            "Delegates.Collections.ProductTypes.FindProductScreenshotsDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public ProductScreenshotsDataController(
            IGetDataAsyncDelegate<List<ProductScreenshots>> getProductScreenshotsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductScreenshots>> postProductScreenshotsDataAsyncDelegate,
            IConvertDelegate<ProductScreenshots, long> convertProductScreenshotsToIndexDelegate,
            IRecordsController<long> productScreenshotsRecordsIndexController,
            IFindDelegate<ProductScreenshots> findProductScreenshotsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getProductScreenshotsDataAsyncDelegate,
                postProductScreenshotsDataAsyncDelegate,
                convertProductScreenshotsToIndexDelegate,
                productScreenshotsRecordsIndexController,
                findProductScreenshotsDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}