using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.Records;

namespace Controllers.Stash.Records
{
    public class ProductScreenshotsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetProductScreenshotsRecordsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProductScreenshotsRecordsStashController(
            IGetPathDelegate getProductScreenshotsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getProductScreenshotsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}