using System;
using System.Threading.Tasks;
using System.Text;

using Interfaces.Delegates.Trace;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Stream;
using Interfaces.Controllers.File;

using Interfaces.Status;

using ProtoBuf;

namespace Controllers.SerializedStorage
{
    public class ProtoBufSerializedStorageController : ISerializedStorageController
    {
        readonly IFileController fileController;
        readonly IStreamController streamController;
        readonly IStatusController statusController;
        readonly ITraceDelegate traceDelegate;

        public ProtoBufSerializedStorageController(
            IFileController fileController,
            IStreamController streamController,
            IStatusController statusController,
            ITraceDelegate traceDelegate = null)
        {
            this.fileController = fileController;
            this.streamController = streamController;
            this.statusController = statusController;

            this.traceDelegate = traceDelegate;
        }

        public async Task<T> DeserializePullAsync<T>(string uri, IStatus status)
        {
            var started = DateTime.Now;
            var deserializePullTask = await statusController.CreateAsync(status, "Reading serialized data", false);

            T data = default(T);

            if (fileController.Exists(uri))
            {
                using (var readableStream = streamController.OpenReadable(uri))
                    data = Serializer.Deserialize<T>(readableStream);
            }

            await statusController.CompleteAsync(deserializePullTask, false);

            var completed = DateTime.Now;
            var duration = (completed - started).TotalMilliseconds;
            traceDelegate?.Trace(
                "DePull",
                started.ToFileTimeUtc().ToString(),
                completed.ToFileTimeUtc().ToString(),
                duration.ToString(),
                uri);

            return data;
        }

        public async Task SerializePushAsync<T>(string uri, T data, IStatus status)
        {
            var started = DateTime.Now;
            var serializePushTask = await statusController.CreateAsync(status, "Writing serialized data", false);

            using (var writableStream = streamController.OpenWritable(uri))
                Serializer.Serialize<T>(writableStream, data);

            await statusController.CompleteAsync(serializePushTask, false);

            var completed = DateTime.Now;
            var duration = (completed - started).TotalMilliseconds;
            traceDelegate?.Trace(
                "DePush",
                started.ToFileTimeUtc().ToString(),
                completed.ToFileTimeUtc().ToString(),
                duration.ToString(),
                uri);
        }
    }
}
