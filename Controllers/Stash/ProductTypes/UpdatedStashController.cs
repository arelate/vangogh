using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.Dependencies;

namespace Controllers.Stash.ProductTypes
{
    public class UpdatedStashController : StashController<List<long>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetUpdatedPathDelegate,Delegates",
            Dependencies.DefaultSerializationController,
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