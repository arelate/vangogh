using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;


namespace Controllers.Stash.ProductTypes
{
    public class UpdatedStashController : StashController<List<long>>
    {
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