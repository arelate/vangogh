using System.Collections.Generic;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class AccountProductsRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.Data.Storage.Records.GetListAccountProductsRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.Records.PostListAccountProductsRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public AccountProductsRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getAccountProductsRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postAccountProductsRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getAccountProductsRecordsDataAsyncDelegate,
                postAccountProductsRecordsDataAsyncDelegate,
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