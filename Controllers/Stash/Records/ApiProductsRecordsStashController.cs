using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class ApiProductsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetApiProductsRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
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