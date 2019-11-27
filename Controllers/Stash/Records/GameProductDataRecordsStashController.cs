using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class GameProductDataRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetGameProductDataRecordsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public GameProductDataRecordsStashController(
            IGetPathDelegate getGameProductDataRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getGameProductDataRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}