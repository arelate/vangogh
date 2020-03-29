using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

namespace Controllers.Data.ProductTypes
{
    public class WishlistedDataController : DataController<long>
    {
        [Dependencies(
            "Delegates.GetData.Storage.ProductTypes.GetListWishlistedDataFromPathAsyncDelegate,Delegates",
            "Delegates.PostData.Storage.ProductTypes.PostListWishlistedDataToPathAsyncDelegate,Delegate",
            "Delegates.Convert.ConvertPassthroughIndexDelegate,Delegates",
            "Controllers.Records.ProductTypes.WishlistedRecordsIndexController,Controllers",
            "Delegates.Find.System.FindLongDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public WishlistedDataController(
            IGetDataAsyncDelegate<List<long>> getWishlistedDataAsyncDelegate,
            IPostDataAsyncDelegate<List<long>> postWishlistedDataAsyncDelegate,
            IConvertDelegate<long, long> convertPassthroughIndexDelegate,
            IRecordsController<long> wishlistedRecordsIndexController,
            IFindDelegate<long> findLongDelegate,
            IActionLogController actionLogController) :
            base(
                getWishlistedDataAsyncDelegate,
                postWishlistedDataAsyncDelegate,
                convertPassthroughIndexDelegate,
                wishlistedRecordsIndexController,
                findLongDelegate,
                actionLogController)
        {
            // ...
        }
    }
}