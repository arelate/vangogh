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
    public class ValidationResultsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.GetData.Storage.Records.GetListValidationResultsRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.Records.PostListValidationResultsRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Find.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ValidationResultsRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getValidationResultsRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postValidationResultsRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IActionLogController actionLogController) :
            base(
                getValidationResultsRecordsDataAsyncDelegate,
                postValidationResultsRecordsDataAsyncDelegate,
                convertProductRecordsToIndexDelegate,
                null,
                findProductRecordsDelegate,
                actionLogController)
        {
            // ...
        }
    }
}