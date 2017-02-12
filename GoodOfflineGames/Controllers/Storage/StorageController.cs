using System.Threading.Tasks;
using System.IO;

using Interfaces.Stream;
using Interfaces.File;
using Interfaces.Storage;

namespace Controllers.Storage
{
    public class StorageController : IStorageController<string>
    {
        private IStreamController streamController;
        private IFileController fileController;

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
            var semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
            await semaphoreSlim.WaitAsync();
            using (var stream = streamController.OpenWritable(uri))
            using (StreamWriter writer = new StreamWriter(stream))
                await writer.WriteLineAsync(data.ToString());
            semaphoreSlim.Release();
        }

        public async Task<string> PullAsync(string uri)
        {
            var semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
            await semaphoreSlim.WaitAsync();

            var data = string.Empty;

            if (fileController.Exists(uri))
            {

                using (var stream = streamController.OpenReadable(uri))
                using (StreamReader reader = new StreamReader(stream))
                    data = await reader.ReadToEndAsync();
            }

            semaphoreSlim.Release();

            return data;
        }
    }
}
