using System.Threading.Tasks;

using Interfaces.Controllers.Hashes;
using Interfaces.Controllers.Storage;
using Interfaces.Controllers.Serialization;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

namespace Controllers.SerializedStorage.JSON
{
    public class JSONSerializedStorageController: SerializedStorageController
    {
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