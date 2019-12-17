using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class AccountProductsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetAccountProductsRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
        public AccountProductsRecordsStashController(
            IGetPathDelegate getAccountProductsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getAccountProductsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}