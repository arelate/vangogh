using System.Collections.Generic;

using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameProductDataDataController : DataController<GameProductData>
    {
        [Dependencies(
            "GOG.Delegates.GetData.Storage.ProductTypes.GetListGameProductDataDataFromPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.PostData.Storage.ProductTypes.PostListGameProductDataDataToPathAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameProductDataToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.GameProductDataRecordsIndexController,Controllers",
            "GOG.Delegates.Find.ProductTypes.FindGameProductDataDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameProductDataDataController(
            IGetDataAsyncDelegate<List<GameProductData>> getListGameProductDataDataAsyncDelegate,
            IPostDataAsyncDelegate<List<GameProductData>> postListGameProductDataDataAsyncDelegate,
            IConvertDelegate<GameProductData, long> convertGameProductDataToIndexDelegate,
            IRecordsController<long> gameProductDataRecordsIndexController,
            IFindDelegate<GameProductData> findGameProductDataDelegate,
            IActionLogController actionLogController) :
            base(
                getListGameProductDataDataAsyncDelegate,
                postListGameProductDataDataAsyncDelegate,
                convertGameProductDataToIndexDelegate,
                gameProductDataRecordsIndexController,
                findGameProductDataDelegate,
                actionLogController)
        {
            // ...
        }
    }
}