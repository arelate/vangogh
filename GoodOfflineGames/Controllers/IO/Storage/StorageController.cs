using System.Threading.Tasks;
using System.IO;

using Interfaces.IO.Stream;
using Interfaces.IO.File;
using Interfaces.IO.Storage;

namespace Controllers.Storage
{
    public class StorageController: IStorageController<string>
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

        public async Task Push(
            string uri,
            string data)
        {
            using (var stream = streamController.OpenWritable(uri))
                using (StreamWriter writer = new StreamWriter(stream))
                    await writer.WriteLineAsync(data);
        }

        public async Task<string> Pull(
            string uri)
        {
            string data = string.Empty;

            if (!fileController.Exists(uri)) return data;

            using (var stream = streamController.OpenReadable(uri))
                using (StreamReader reader = new StreamReader(stream))
                    data = await reader.ReadToEndAsync();

            return data;
        }
    }
}
