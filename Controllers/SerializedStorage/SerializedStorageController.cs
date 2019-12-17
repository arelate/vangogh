using System.Threading.Tasks;

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
        IStatusController statusController;

        public SerializedStorageController(
            IStorageController<string> storageController,
            ISerializationController<string> serializarionController,
            IStatusController statusController)
        {
            this.storageController = storageController;
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
            await storageController.PushAsync(uri, serializedData);
        }
    }
}
