using System.Threading.Tasks;
using System.Text;

using Interfaces.SerializedStorage;
using Interfaces.Storage;
using Interfaces.Serialization;
using Interfaces.Hash;

namespace Controllers.SerializedStorage
{
    public class SerializedStorageController : ISerializedStorageController
    {
        private IStorageController<string> storageController;
        private ISerializationController<string> serializarionController;
        private IStringHashController stringHashController;
        private IPrecomputedHashController precomputedHashController;

        public SerializedStorageController(
            IPrecomputedHashController precomputedHashController,
            IStorageController<string> storageController,
            IStringHashController stringHashController,
            ISerializationController<string> serializarionController)
        {
            this.precomputedHashController = precomputedHashController;
            this.storageController = storageController;
            this.stringHashController = stringHashController;
            this.serializarionController = serializarionController;
        }

        public async Task<T> DeserializePullAsync<T>(string uri)
        {
            var serializedData = await storageController.PullAsync(uri);

            //var hash = stringHashController.GetHash(serializedData);
            //await precomputedHashController.SetHashAsync(uri, hash);

            return serializarionController.Deserialize<T>(serializedData);
        }

        public async Task SerializePushAsync<T>(string uri, T data)
        {
            var serializedData = serializarionController.Serialize(data);

            var hash = stringHashController.GetHash(serializedData);
            var existingHash = precomputedHashController.GetHash(uri);

            // data has not changed, no need to write to storage
            if (hash == existingHash) return;

            await precomputedHashController.SetHashAsync(uri, hash);
            await storageController.PushAsync(uri, serializedData);
        }
    }
}
