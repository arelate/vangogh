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
    public class ProductDownloadsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListProductDownloadsRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.Records.PostListProductDownloadsRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Find.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductDownloadsRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getProductDownloadsRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postProductDownloadsRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IActionLogController actionLogController) :
            base(
                getProductDownloadsRecordsDataAsyncDelegate,
                postProductDownloadsRecordsDataAsyncDelegate,
                convertProductRecordsToIndexDelegate,
                null,
                findProductRecordsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}