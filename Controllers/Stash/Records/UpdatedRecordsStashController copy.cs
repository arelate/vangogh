using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class WishlistedRecordsStashController : StashController<List<ProductRecords>>
    {
        public WishlistedRecordsStashController(
            IGetPathDelegate getWishlistedRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getWishlistedRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}