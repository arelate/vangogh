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
    public class ProductRoutesRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListProductRoutesRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.Records.PostListProductRoutesRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Find.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProductRoutesRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getProductRoutesRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postProductRoutesRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IActionLogController actionLogController) :
            base(
                getProductRoutesRecordsDataAsyncDelegate,
                postProductRoutesRecordsDataAsyncDelegate,
                convertProductRecordsToIndexDelegate,
                null,
                findProductRecordsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}