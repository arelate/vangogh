using System;
using System.Threading.Tasks;
using System.Text;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Stream;
using Interfaces.Controllers.File;

using Interfaces.Status;

using Attributes;

using ProtoBuf;

namespace Controllers.SerializedStorage.ProtoBuf
{
    public class ProtoBufSerializedStorageController : ISerializedStorageController
    {
        readonly IFileController fileController;
        readonly IStreamController streamController;
        readonly IStatusController statusController;

        [Dependencies(
            "Controllers.File.FileController,Controllers",
            "Controllers.Stream.StreamController,Controllers",
            "Controllers.Status.StatusController,Controllers")]
        public ProtoBufSerializedStorageController(
            IFileController fileController,
            IStreamController streamController,
            IStatusController statusController)
        {
            this.fileController = fileController;
            this.streamController = streamController;
            this.statusController = statusController;
        }

        public async Task<T> DeserializePullAsync<T>(string uri, IStatus status)
        {
            var deserializePullTask = await statusController.CreateAsync(status, "Reading serialized data", false);

            T data = default(T);

            if (fileController.Exists(uri))
            {
                using (var readableStream = streamController.OpenReadable(uri))
                    data = Serializer.Deserialize<T>(readableStream);
            }

            await statusController.CompleteAsync(deserializePullTask, false);

            return data;
        }

        public async Task SerializePushAsync<T>(string uri, T data, IStatus status)
        {
            var serializePushTask = await statusController.CreateAsync(status, "Writing serialized data", false);

            using (var writableStream = streamController.OpenWritable(uri))
                Serializer.Serialize<T>(writableStream, data);

            await statusController.CompleteAsync(serializePushTask, false);
        }
    }
}
