using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Controllers.Stash;

using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class AccountProductsStashController : StashController<List<AccountProduct>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetAccountProductsPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public AccountProductsStashController(
            IGetPathDelegate getAccountProductsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getAccountProductsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}