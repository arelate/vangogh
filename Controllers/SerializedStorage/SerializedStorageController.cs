using System.Threading.Tasks;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Storage;
using Interfaces.Controllers.Serialization;
using Interfaces.Controllers.Logs;

namespace Controllers.SerializedStorage
{
    public abstract class SerializedStorageController : ISerializedStorageController
    {
        readonly IStorageController<string> storageController;
        readonly ISerializationController<string> serializarionController;
        IActionLogController actionLogController;

        public SerializedStorageController(
            IStorageController<string> storageController,
            ISerializationController<string> serializarionController,
            IActionLogController actionLogController)
        {
            this.storageController = storageController;
            this.serializarionController = serializarionController;
            this.actionLogController = actionLogController;
        }

        public async Task<T> DeserializePullAsync<T>(string uri)
        {
            var serializedData = await storageController.PullAsync(uri);
            return serializarionController.Deserialize<T>(serializedData);
        }

        public async Task SerializePushAsync<T>(string uri, T data)
        {
            var serializedData = serializarionController.Serialize(data);
            await storageController.PushAsync(uri, serializedData);
        }
    }
}
