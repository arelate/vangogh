using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;

using Attributes;

using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class WishlistedStashController : StashController<List<long>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetWishlistedPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public WishlistedStashController(
            IGetPathDelegate getWishlistedPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getWishlistedPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}