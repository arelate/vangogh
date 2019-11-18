using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class ApiProductsRecordsStashController : StashController<Dictionary<long, ProductRecords>>
    {
        public ApiProductsRecordsStashController(
            IGetPathDelegate getApiProductsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getApiProductsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}