using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class GameDetailsRecordsStashController : StashController<Dictionary<long, ProductRecords>>
    {
        public GameDetailsRecordsStashController(
            IGetPathDelegate getGameDetailsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getGameDetailsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}