using System.Collections.Generic;
using Interfaces.Controllers.Records;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Activities;
using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class WishlistedDataController : DataController<long>
    {
        [Dependencies(
            "Delegates.Data.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate,Delegates",
            "Delegates.Data.Storage.ProductTypes.PostListWishlistedDataToPathAsyncDelegate,Delegates",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.WishlistedRecordsIndexController,Controllers",
            "Delegates.Collections.System.FindLongDelegate,Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public WishlistedDataController(
            IGetDataAsyncDelegate<List<long>> getWishlistedDataAsyncDelegate,
            IPostDataAsyncDelegate<List<long>> postWishlistedDataAsyncDelegate,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> wishlistedRecordsIndexController,
            IFindDelegate<long> findLongDelegate,
            IStartDelegate startDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                getWishlistedDataAsyncDelegate,
                postWishlistedDataAsyncDelegate,
                convertPassthroughIndexDelegate,
                wishlistedRecordsIndexController,
                findLongDelegate,
                startDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}