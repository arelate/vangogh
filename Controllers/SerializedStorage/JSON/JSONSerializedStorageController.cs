using System.Threading.Tasks;

using Interfaces.Controllers.Storage;
using Interfaces.Controllers.Serialization;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Attributes;

using Models.Dependencies;

namespace Controllers.SerializedStorage.JSON
{
    public class JSONSerializedStorageController: SerializedStorageController
    {
        [Dependencies(
            "Controllers.Storage.StorageController,Controllers",
            Dependencies.JSONSerializationController,
            "Controllers.Logs.ActionLogController,Controllers")]
        public JSONSerializedStorageController(
            IStorageController<string> storageController,
            ISerializationController<string> jsonSerializarionController,
            IActionLogController actionLogController):
            base(
                storageController,
                jsonSerializarionController,
                actionLogController)
            {
                // ...
            }
    }
}