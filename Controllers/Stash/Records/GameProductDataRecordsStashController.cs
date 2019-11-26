using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class GameProductDataRecordsStashController : StashController<List<ProductRecords>>
    {
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