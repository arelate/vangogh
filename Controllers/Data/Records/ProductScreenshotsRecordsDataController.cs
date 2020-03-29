using System.Collections.Generic;

using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class ProductScreenshotsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListProductScreenshotsRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.Records.PostListProductScreenshotsRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Find.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductScreenshotsRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getProductScreenshotsRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postProductScreenshotsRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IActionLogController actionLogController) :
            base(
                getProductScreenshotsRecordsDataAsyncDelegate,
                postProductScreenshotsRecordsDataAsyncDelegate,
                convertProductRecordsToIndexDelegate,
                null,
                findProductRecordsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}