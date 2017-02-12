using System.Threading.Tasks;

using Interfaces.SerializedStorage;
using Interfaces.Storage;
using Interfaces.Serialization;
using Interfaces.Conversion;

namespace Controllers.SerializedStorage
{
    public class SerializedStorageController : ISerializedStorageController
    {
        private IStorageController<string> storageController;
        private ISerializationController<string> serializarionController;

        public SerializedStorageController(
            IStorageController<string> storageController,
            ISerializationController<string> serializarionController)
        {
            this.storageController = storageController;
            this.serializarionController = serializarionController;
        }

        public async Task<T> DeserializePullAsync<T>(string uri)
        {
            var serializedData = await storageController.Pull(uri);
            return serializarionController.Deserialize<T>(serializedData);
        }

        public async Task SerializePushAsync<T>(string uri, T data)
        {
            var serializedData = serializarionController.Serialize(data);
            await storageController.Push(uri, serializedData);
        }
    }
}
