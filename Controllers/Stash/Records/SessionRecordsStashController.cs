using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class SessionRecordsStashController : StashController<List<ProductRecords>>
    {
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