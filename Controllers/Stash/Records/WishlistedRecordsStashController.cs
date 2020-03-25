using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Logs;


using Attributes;

using Models.ProductTypes;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class WishlistedRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetWishlistedPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public WishlistedRecordsStashController(
            IGetPathDelegate getWishlistedRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IActionLogController actionLogController) :
            base(
                getWishlistedRecordsPathDelegate,
                serializedStorageController,
                actionLogController)
        {
            // ...
        }
    }
}