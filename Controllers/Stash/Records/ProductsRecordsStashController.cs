using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.Records;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class ProductsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetProductsRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
        public ProductsRecordsStashController(
            IGetPathDelegate getProductsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getProductsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}