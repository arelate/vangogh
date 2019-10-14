using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

namespace Controllers.Stash.Hashes
{
    public class HashesStashController : StashController<Dictionary<string, string>>
    {
        public HashesStashController(
            IGetPathDelegate getHashesPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getHashesPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}