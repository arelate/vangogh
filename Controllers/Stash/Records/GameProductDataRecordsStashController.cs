using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class GameProductDataRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetGameProductDataRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public GameProductDataRecordsStashController(
            IGetPathDelegate getGameProductDataRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getGameProductDataRecordsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}