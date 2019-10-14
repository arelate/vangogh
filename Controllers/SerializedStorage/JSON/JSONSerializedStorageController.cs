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
            IHashesController hashesController,
            IStorageController<string> storageController,
            IConvertAsyncDelegate<string, Task<string>> convertStringToHashDelegate,
            ISerializationController<string> jsonSerializarionController,
            IStatusController statusController):
            base(
                hashesController,
                storageController,
                convertStringToHashDelegate,
                jsonSerializarionController,
                statusController)
            {
                // ...
            }
    }
}