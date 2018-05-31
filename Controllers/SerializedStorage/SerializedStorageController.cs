using System.Threading.Tasks;
using System.Text;

using Interfaces.Delegates.Hash;

using Interfaces.Controllers.Hash;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Storage;
using Interfaces.Controllers.Serialization;

using Interfaces.Status;

namespace Controllers.SerializedStorage
{
    public class SerializedStorageController : ISerializedStorageController
    {
        readonly IStorageController<string> storageController;
        readonly ISerializationController<string> serializarionController;
        readonly IGetHashAsyncDelegate<string> getStringHashAsyncDelegate;
        readonly IStoredHashController storedHashController;
        IStatusController statusController;

        public SerializedStorageController(
            IStoredHashController storedHashController,
            IStorageController<string> storageController,
            IGetHashAsyncDelegate<string> getStringHashAsyncDelegate,
            ISerializationController<string> serializarionController,
            IStatusController statusController)
        {
            this.storedHashController = storedHashController;
            this.storageController = storageController;
            this.getStringHashAsyncDelegate = getStringHashAsyncDelegate;
            this.serializarionController = serializarionController;
            this.statusController = statusController;
        }

        public async Task<T> DeserializePullAsync<T>(string uri, IStatus status)
        {
            var serializedData = await storageController.PullAsync(uri);

            return serializarionController.Deserialize<T>(serializedData);
        }

        public async Task SerializePushAsync<T>(string uri, T data, IStatus status)
        {
            var serializedData = serializarionController.Serialize(data);

            var hash = await getStringHashAsyncDelegate.GetHashAsync(serializedData, status);
            var existingHash = await storedHashController.GetHashAsync(uri, status);

            // data has not changed, no need to write to storage
            if (hash == existingHash) return;

            await storedHashController.SetHashAsync(uri, hash, status);
            await storageController.PushAsync(uri, serializedData);
        }
    }
}
