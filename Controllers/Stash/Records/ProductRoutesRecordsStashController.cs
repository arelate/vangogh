using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.Records;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class ProductRoutesRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetProductRoutesRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
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