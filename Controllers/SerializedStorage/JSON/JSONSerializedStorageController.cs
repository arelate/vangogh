using System.Threading.Tasks;

using Interfaces.Controllers.Storage;
using Interfaces.Controllers.Serialization;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.Dependencies;

namespace Controllers.SerializedStorage.JSON
{
    public class JSONSerializedStorageController: SerializedStorageController
    {
        [Dependencies(
            "Controllers.Storage.StorageController,Controllers",
            Dependencies.JSONSerializationController,
            "Controllers.Status.StatusController,Controllers")]
        public JSONSerializedStorageController(
            IStorageController<string> storageController,
            ISerializationController<string> jsonSerializarionController,
            IStatusController statusController):
            base(
                storageController,
                jsonSerializarionController,
                statusController)
            {
                // ...
            }
    }
}