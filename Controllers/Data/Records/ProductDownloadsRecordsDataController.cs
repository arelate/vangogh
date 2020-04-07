using System.Collections.Generic;

using Interfaces.Delegates.Activities;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class ProductDownloadsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.Data.Storage.Records.GetListProductDownloadsRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.Records.PostListProductDownloadsRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public ProductDownloadsRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getProductDownloadsRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postProductDownloadsRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getProductDownloadsRecordsDataAsyncDelegate,
                postProductDownloadsRecordsDataAsyncDelegate,
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