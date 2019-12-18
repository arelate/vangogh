using System;
using System.Threading.Tasks;
using System.Text;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Stream;
using Interfaces.Controllers.File;
using Interfaces.Controllers.Logs;

using Attributes;

using ProtoBuf;

namespace Controllers.SerializedStorage.ProtoBuf
{
    public class ProtoBufSerializedStorageController : ISerializedStorageController
    {
        readonly IFileController fileController;
        readonly IStreamController streamController;
        readonly IActionLogController actionLogController;

        [Dependencies(
            "Controllers.File.FileController,Controllers",
            "Controllers.Stream.StreamController,Controllers",
            "Controllers.Logs.ResponseLogController,Controllers")]
        public ProtoBufSerializedStorageController(
            IFileController fileController,
            IStreamController streamController,
            IActionLogController actionLogController)
        {
            this.fileController = fileController;
            this.streamController = streamController;
            this.actionLogController = actionLogController;
        }

        // TODO: Make async
        public async Task<T> DeserializePullAsync<T>(string uri)
        {
            actionLogController.StartAction("Reading serialized data");

            T data = default(T);

            if (fileController.Exists(uri))
            {
                using (var readableStream = streamController.OpenReadable(uri))
                    data = Serializer.Deserialize<T>(readableStream);
            }

            actionLogController.CompleteAction();

            return data;
        }

        // TODO: Make async
        public async Task SerializePushAsync<T>(string uri, T data)
        {
            actionLogController.StartAction("Writing serialized data");

            using (var writableStream = streamController.OpenWritable(uri))
                Serializer.Serialize<T>(writableStream, data);

            actionLogController.CompleteAction();
        }
    }
}
