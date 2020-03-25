using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;


using Attributes;

using Controllers.Data;

using GOG.Models;

namespace GOG.Controllers.Data.ProductTypes
{
    public class GameProductDataDataController : DataController<GameProductData>
    {
        [Dependencies(
            "GOG.Controllers.Stash.ProductTypes.GameProductDataStashController,GOG.Controllers",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameProductDataToIndexDelegate,GOG.Delegates",
            "Controllers.Records.ProductTypes.GameProductDataRecordsIndexController,Controllers",
            "GOG.Delegates.Find.ProductTypes.FindGameProductDataDelegate,GOG.Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameProductDataDataController(
            IStashController<List<GameProductData>> gameProductDataStashController,
            IConvertDelegate<GameProductData, long> convertGameProductDataToIndexDelegate,
            IRecordsController<long> gameProductDataRecordsIndexController,
            IFindDelegate<GameProductData> findGameProductDataDelegate,
            IActionLogController actionLogController) :
            base(
                gameProductDataStashController,
                convertGameProductDataToIndexDelegate,
                gameProductDataRecordsIndexController,
                findGameProductDataDelegate,
                actionLogController)
        {
            // ...
        }
    }
}