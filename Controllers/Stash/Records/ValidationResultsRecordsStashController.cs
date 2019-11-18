using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class ValidationResultsRecordsStashController : StashController<Dictionary<long, ProductRecords>>
    {
        public ValidationResultsRecordsStashController(
            IGetPathDelegate getValidationResultsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getValidationResultsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}