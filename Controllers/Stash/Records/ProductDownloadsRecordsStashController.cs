using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class ProductDownloadsRecordsStashController : StashController<List<ProductRecords>>
    {
        public ProductDownloadsRecordsStashController(
            IGetPathDelegate getProductDownloadsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getProductDownloadsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}