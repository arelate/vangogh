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
        private IHashTrackingController hashTrackingController;

        public SerializedStorageController(
            IHashTrackingController hashTrackingController,
            IStorageController<string> storageController,
            ISerializationController<string> serializarionController)
        {
            this.hashTrackingController = hashTrackingController;
            this.storageController = storageController;
            this.serializarionController = serializarionController;
        }

        public async Task<T> DeserializePullAsync<T>(string uri)
        {
            var serializedData = await storageController.PullAsync(uri);

            var hash = serializedData.GetHashCode();
            await hashTrackingController.SetHashAsync(uri, hash);

            return serializarionController.Deserialize<T>(serializedData);
        }

        public async Task SerializePushAsync<T>(string uri, T data)
        {
            var serializedData = serializarionController.Serialize(data);

            var hash = serializedData.GetHashCode();
            var lastKnownHash = hashTrackingController.GetHash(uri);

            // data has not changed, no need to write to storage
            if (hash == lastKnownHash) return;

            await hashTrackingController.SetHashAsync(uri, hash);
            await storageController.PushAsync(uri, serializedData);
        }
    }
}
