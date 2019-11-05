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

        public StorageController(
            IStreamController streamController,
            IFileController fileController)
        {
            this.streamController = streamController;
            this.fileController = fileController;
        }

        public async Task PushAsync(
            string uri,
            string data)
        {
            var started = DateTime.Now;

            using (var stream = streamController.OpenWritable(uri))
            using (StreamWriter writer = new StreamWriter(stream))
                await writer.WriteLineAsync(data);

            var completed = DateTime.Now;
            var duration = (completed - started).TotalMilliseconds;
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

            var completed = DateTime.Now;
            var duration = (completed - started).TotalMilliseconds;

            return data;
        }
    }
}
