using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

namespace Controllers.Stash.ProductTypes
{
    public class WishlistedStashController : StashController<List<long>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetWishlistedPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public WishlistedStashController(
            IGetPathDelegate getWishlistedPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getWishlistedPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}