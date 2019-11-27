using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class SessionRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetSessionRecordsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public SessionRecordsStashController(
            IGetPathDelegate getSessionRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getSessionRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}