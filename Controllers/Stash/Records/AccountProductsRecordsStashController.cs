using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class AccountProductsRecordsStashController : StashController<Dictionary<long, ProductRecords>>
    {
        public AccountProductsRecordsStashController(
            IGetPathDelegate getAccountProductsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getAccountProductsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}