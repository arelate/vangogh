using System.Collections.Generic;

using Interfaces.Delegates.Activities;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Attributes;

using Models.ProductTypes;

namespace Controllers.Data.Records
{
    public class WishlistedRecordsDataController : DataController<ProductRecords>
    {
        [Dependencies(
            "Delegates.Data.Storage.Records.GetListWishlistedRecordsDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.Records.PostListWishlistedRecordsDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.Records.ConvertProductRecordsToIndexDelegate,Delegates",
            "Delegates.Collections.ProductTypes.FindProductRecordsDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public WishlistedRecordsDataController(
            IGetDataAsyncDelegate<List<ProductRecords>> getWishlistedRecordsDataAsyncDelegate,
            IPostDataAsyncDelegate<List<ProductRecords>> postWishlistedRecordsDataAsyncDelegate,
            IConvertDelegate<ProductRecords, long> convertProductRecordsToIndexDelegate,
            IFindDelegate<ProductRecords> findProductRecordsDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate):
            base(
                getWishlistedRecordsDataAsyncDelegate,
                postWishlistedRecordsDataAsyncDelegate,
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