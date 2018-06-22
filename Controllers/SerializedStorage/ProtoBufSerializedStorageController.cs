using System.Threading.Tasks;
using System.Text;

// using Interfaces.Delegates.Hash;

// using Interfaces.Controllers.Hash;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Stream;
// using Interfaces.Controllers.Storage;
// using Interfaces.Controllers.Serialization;

using Interfaces.Status;

using ProtoBuf;

namespace Controllers.SerializedStorage
{
    public class ProtoBufSerializedStorageController : ISerializedStorageController
    {
        readonly IStreamController streamController;
        readonly IStatusController statusController;

        public ProtoBufSerializedStorageController(
            IStreamController streamController,
            IStatusController statusController)
        {
            this.streamController = streamController;
            this.statusController = statusController;
        }

        public async Task<T> DeserializePullAsync<T>(string uri, IStatus status)
        {
            var deserializePullTask = await statusController.CreateAsync(status, "Reading serialized data");

            T data = default(T);
            using (var readableStream = streamController.OpenReadable(uri))
                data = Serializer.Deserialize<T>(readableStream);

            await statusController.CompleteAsync(deserializePullTask);

            return data;
        }

        public async Task SerializePushAsync<T>(string uri, T data, IStatus status)
        {
            var serializePushTask = await statusController.CreateAsync(status, "Writing serialized data");

            using (var writableStream = streamController.OpenWritable(uri))
                Serializer.Serialize<T>(writableStream, data);

            await statusController.CompleteAsync(serializePushTask);
        }
    }
}
