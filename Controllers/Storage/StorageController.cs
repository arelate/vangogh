using System;
using System.Threading.Tasks;
using System.IO;

using Interfaces.Delegates.Trace;

using Interfaces.Controllers.File;
using Interfaces.Controllers.Stream;
using Interfaces.Controllers.Storage;

namespace Controllers.Storage
{
    public class StorageController : IStorageController<string>
    {
        readonly IStreamController streamController;
        readonly IFileController fileController;
        readonly ITraceDelegate traceDelegate;

        public StorageController(
            IStreamController streamController,
            IFileController fileController,
            ITraceDelegate traceDelegate = null)
        {
            this.streamController = streamController;
            this.fileController = fileController;
            this.traceDelegate = traceDelegate;
        }

        public async Task PushAsync(
            string uri,
            string data)
        {
            var started = DateTime.Now;

            using (var stream = streamController.OpenWritable(uri))
            using (StreamWriter writer = new StreamWriter(stream))
                await writer.WriteLineAsync(data);

            if (traceDelegate == null) return;

            var completed = DateTime.Now;
            var duration = (completed - started).TotalMilliseconds;
            traceDelegate.Trace(
                "Push",
                started.ToFileTimeUtc().ToString(),
                completed.ToFileTimeUtc().ToString(),
                duration.ToString(),
                uri);
        }

        public async Task<string> PullAsync(string uri)
        {
            var started = DateTime.Now;
            var data = string.Empty;

            if (fileController.Exists(uri))
            {

                using (var stream = streamController.OpenReadable(uri))
                using (StreamReader reader = new StreamReader(stream))
                    data = await reader.ReadToEndAsync();
            }

            if (traceDelegate == null) return data;

            var completed = DateTime.Now;
            var duration = (completed - started).TotalMilliseconds;
            traceDelegate.Trace(
                "Pull",
                started.ToFileTimeUtc().ToString(),
                completed.ToFileTimeUtc().ToString(),
                duration.ToString(),
                uri);

            return data;
        }
    }
}
