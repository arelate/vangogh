using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class ProductRoutesRecordsStashController : StashController<List<ProductRecords>>
    {
        public ProductRoutesRecordsStashController(
            IGetPathDelegate getProductRoutesRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getProductRoutesRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}