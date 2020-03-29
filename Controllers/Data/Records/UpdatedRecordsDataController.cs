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
    public class UpdatedRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListUpdatedRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.Records.PostListUpdatedRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Find.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdatedRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getUpdatedRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postUpdatedRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IActionLogController actionLogController) :
            base(
                getUpdatedRecordsDataAsyncDelegate,
                postUpdatedRecordsDataAsyncDelegate,
                convertProductRecordsToIndexDelegate,
                null,
                findProductRecordsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}