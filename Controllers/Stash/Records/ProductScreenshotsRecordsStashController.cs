using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class ProductScreenshotsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetProductScreenshotsRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
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