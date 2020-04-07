using System.Collections.Generic;

using Interfaces.Delegates.Activities;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class ProductScreenshotsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.Data.Storage.Records.GetListProductScreenshotsRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.Records.PostListProductScreenshotsRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public ProductScreenshotsRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getProductScreenshotsRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postProductScreenshotsRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getProductScreenshotsRecordsDataAsyncDelegate,
                postProductScreenshotsRecordsDataAsyncDelegate,
                convertProductRecordsToIndexDelegate,
                null,
                findProductRecordsDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}