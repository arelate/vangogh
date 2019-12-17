using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class ProductDownloadsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetProductDownloadsRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
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