using System.Threading.Tasks;
using System.IO;

using Interfaces.Controllers.SerializedStorage;
using Interfaces.Delegates.Convert;
using Interfaces.Controllers.Logs;
using Interfaces.Models.Dependencies;

using Attributes;

using ProtoBuf;

namespace Controllers.SerializedStorage.ProtoBuf
{
    public class ProtoBufSerializedStorageController<T> : ISerializedStorageController<T>
    {
        private readonly IConvertDelegate<string, System.IO.Stream> convertUriToReadableStreamDelegate;
        private readonly IConvertDelegate<string, System.IO.Stream> convertUriToWritableStreamDelegate;
        readonly IActionLogController actionLogController;

        [Dependencies(
            DependencyContext.Default,
            "Delegates.Convert.Streams.ConvertUriToReadableStreamDelegate,Delegates",
            "Delegates.Convert.Streams.ConvertUriToWritableDelegate,Delegates",
            "Controllers.Logs.ActionLogController,Controllers")]
        public ProtoBufSerializedStorageController(
            IConvertDelegate<string, System.IO.Stream> convertUriToReadableStreamDelegate,
            IConvertDelegate<string, System.IO.Stream> convertUriToWritableStreamDelegate,
            IActionLogController actionLogController)
        {
            this.convertUriToReadableStreamDelegate = convertUriToReadableStreamDelegate;
            this.convertUriToWritableStreamDelegate = convertUriToWritableStreamDelegate;
            this.actionLogController = actionLogController;
        }

        // TODO: Make async
        public async Task<T> DeserializePullAsync(string uri)
        {
            actionLogController.StartAction("Reading serialized data");

            T data = default(T);

            if (System.IO.File.Exists(uri))
            {
                using (var readableStream = convertUriToReadableStreamDelegate.Convert(uri))
                    data = Serializer.Deserialize<T>(readableStream);
            }

            actionLogController.CompleteAction();

            return data;
        }

        // TODO: Make async
        public async Task SerializePushAsync(string uri, T data)
        {
            actionLogController.StartAction("Writing serialized data");

            using (var writableStream = convertUriToWritableStreamDelegate.Convert(uri))
                Serializer.Serialize<T>(writableStream, data);

            actionLogController.CompleteAction();
        }
    }
}
