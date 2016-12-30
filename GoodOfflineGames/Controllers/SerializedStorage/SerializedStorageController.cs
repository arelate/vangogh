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
        private IConversionController<string, string> stringConversionController;

        public SerializedStorageController(
            IStorageController<string> storageController,
            ISerializationController<string> serializarionController,
            IConversionController<string, string> stringConversionController)
        {
            this.storageController = storageController;
            this.serializarionController = serializarionController;
            this.stringConversionController = stringConversionController;
        }

        public async Task<T> DeserializePullAsync<T>(string uri)
        {
            var serializedData = await storageController.Pull(uri);
            if (stringConversionController != null)
                serializedData = stringConversionController.Convert(serializedData);
            return serializarionController.Deserialize<T>(serializedData);
        }

        public async Task SerializePushAsync<T>(string uri, T data)
        {
            var serializedData = serializarionController.Serialize(data);
            if (stringConversionController != null)
                serializedData = stringConversionController.Convert(serializedData);
            await storageController.Push(uri, serializedData);
        }
    }
}
