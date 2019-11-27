using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

namespace Controllers.Stash.ProductTypes
{
    public class UpdatedStashController : StashController<List<long>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate,Delegates",
            "Controllers.SerializedStorage.JSON.JSONSerializedStorageController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public UpdatedStashController(
            IGetPathDelegate getUpdatedPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getUpdatedPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}