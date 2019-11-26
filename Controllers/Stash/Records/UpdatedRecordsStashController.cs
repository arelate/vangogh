using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class UpdatedRecordsStashController : StashController<List<ProductRecords>>
    {
        public UpdatedRecordsStashController(
            IGetPathDelegate getUpdatedRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getUpdatedRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}