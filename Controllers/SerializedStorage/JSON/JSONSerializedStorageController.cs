using System.Threading.Tasks;

using Interfaces.Controllers.Storage;
using Interfaces.Controllers.Serialization;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

namespace Controllers.SerializedStorage.JSON
{
    public class JSONSerializedStorageController: SerializedStorageController
    {
        [Dependencies(
            "Controllers.Storage.StorageController,Controllers",
            "Controllers.Serialization.JSON.JSONSerializationController,Controllers",
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