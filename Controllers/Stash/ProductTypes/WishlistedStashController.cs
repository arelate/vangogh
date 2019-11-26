using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;


namespace Controllers.Stash.ProductTypes
{
    public class WishlistedStashController : StashController<List<long>>
    {
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