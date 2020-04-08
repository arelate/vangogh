using System.Collections.Generic;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;
using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class GameProductDataRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.Data.Storage.Records.GetListGameProductDataRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.Records.PostListGameProductDataRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public GameProductDataRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getGameProductDataRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postGameProductDataRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getGameProductDataRecordsDataAsyncDelegate,
                postGameProductDataRecordsDataAsyncDelegate,
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